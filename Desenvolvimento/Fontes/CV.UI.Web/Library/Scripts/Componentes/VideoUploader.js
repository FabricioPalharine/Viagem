/**
 * YouTube video uploader class
 *
 * @constructor
 */
var STATUS_POLLING_INTERVAL_MILLIS = 10 * 1000; // One minute.


var UploadVideo = function () {
   
    this.tags = "";

    this.itemArquivo = {};
    this.videoId = '';

    this.uploadStartTime = 0;

    this.callbackCompleto = null;

    this.thumbnail = "";
    this.MensagemProcessando = "";
    this.callbackUpdate = null;
};


UploadVideo.prototype.ready = function (accessToken, Titulo, Arquivo, callbackFim) {
    this.accessToken = accessToken;
    this.gapi = gapi;
    this.callbackCompleto = callbackFim;
    this.authenticated = true;
    this.gapi.client.request({
        path: '/youtube/v3/channels',
        params: {
            part: 'snippet',
            mine: true
        },
        callback: function (response) {
            if (response.error) {
                callbackFim(null,response.error.message);
            } else {
                this.uploadFile(Titulo, Arquivo);
            }
        }.bind(this)
    });

};

/**
 * Uploads a video file to YouTube.
 *
 * @method uploadFile
 * @param {object} file File object corresponding to the video to upload.
 */
UploadVideo.prototype.uploadFile = function (Titulo, Arquivo) {
    var metadata = {
        snippet: {
            title: Titulo,

        },
        status: {
            privacyStatus: "unlisted"
        }
    };
    var uploader = new MediaUploader({
        baseUrl: 'https://www.googleapis.com/upload/youtube/v3/videos',
        file: Arquivo,
        token: this.accessToken,
        metadata: metadata,
        params: {
            part: Object.keys(metadata).join(',')
        },
        onError: function (data) {
            var message = data;
            // Assuming the error is raised by the YouTube API, data will be
            // a JSON string with error.message set. That may not be the
            // only time onError will be raised, though.
            try {
                var errorResponse = JSON.parse(data);
                message = errorResponse.error.message;
            } finally {
                alert(message);
            }
        }.bind(this),
        onProgress: function (data) {
            var currentTime = Date.now();
            var bytesUploaded = data.loaded;
            var totalBytes = data.total;
            // The times are in millis, so we need to divide by 1000 to get seconds.
            var bytesPerSecond = bytesUploaded / ((currentTime - this.uploadStartTime) / 1000);
            var estimatedSecondsRemaining = (totalBytes - bytesUploaded) / bytesPerSecond;
            var percentageComplete = (bytesUploaded * 100) / totalBytes;
            this.itemArquivo.Porcentual = parseInt(percentageComplete);
            if (this.callbackUpdate)
                this.callbackUpdate();
            //$('#upload-progress').attr({
            //    value: bytesUploaded,
            //    max: totalBytes
            //});

            //$('#percent-transferred').text(percentageComplete);
            //$('#bytes-transferred').text(bytesUploaded);
            //$('#total-bytes').text(totalBytes);

            //$('.during-upload').show();
        }.bind(this),
        onComplete: function (data) {
            var uploadResponse = JSON.parse(data);
            this.videoId = uploadResponse.id;
            this.thumbnail = uploadResponse.snippet.thumbnails.default.url;
            this.itemArquivo.Situacao = this.MensagemProcessando;
            if (this.callbackUpdate)
                this.callbackUpdate();
            //$('#video-id').text(this.videoId);
            //$('.post-upload').show();
            this.pollForVideoStatus();

        }.bind(this)
    });
    this.uploadStartTime = Date.now();
    uploader.upload();
};



UploadVideo.prototype.pollForVideoStatus = function () {
    this.gapi.client.request({
        path: '/youtube/v3/videos',
        params: {
            part: 'status,player',
            id: this.videoId
        },
        callback: function (response) {
            if (response.error) {
                // The status polling failed.
                console.log(response.error.message);
                setTimeout(this.pollForVideoStatus.bind(this), STATUS_POLLING_INTERVAL_MILLIS);
            } else {
                var uploadStatus = "uploaded";
                if (response.items && response.items.length > 0)
                    uploadStatus=response.items[0].status.uploadStatus;
                switch (uploadStatus) {
                    // This is a non-final status, so we need to poll again.
                    case 'uploaded':
                        //$('#post-upload-status').append('<li>Upload status: ' + uploadStatus + '</li>');
                        setTimeout(this.pollForVideoStatus.bind(this), STATUS_POLLING_INTERVAL_MILLIS);
                        break;
                        // The video was successfully transcoded and is available.
                    case 'processed':
                        this.callbackCompleto( $(response.items[0].player.embedHtml).attr("src"));
                        //$('#post-upload-status').append('<li>Final status.</li>');
                        break;
                        // All other statuses indicate a permanent transcoding failure.
                    default:
                        this.callbackCompleto(null, uploadStatus);
                       // $('#post-upload-status').append('<li>Transcoding failed.</li>');
                        break;
                }
            }
        }.bind(this)
    });
};


/*
Copyright 2015 Google Inc. All Rights Reserved.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

  http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

var DRIVE_UPLOAD_URL = 'https://www.googleapis.com/upload/drive/v2/files/';


/**
 * Helper for implementing retries with backoff. Initial retry
 * delay is 1 second, increasing by 2x (+jitter) for subsequent retries
 *
 * @constructor
 */
var RetryHandler = function () {
    this.interval = 1000; // Start at one second
    this.maxInterval = 60 * 1000; // Don't wait longer than a minute 
};

/**
 * Invoke the function after waiting
 *
 * @param {function} fn Function to invoke
 */
RetryHandler.prototype.retry = function (fn) {
    setTimeout(fn, this.interval);
    this.interval = this.nextInterval_();
};

/**
 * Reset the counter (e.g. after successful request.)
 */
RetryHandler.prototype.reset = function () {
    this.interval = 1000;
};

/**
 * Calculate the next wait time.
 * @return {number} Next wait interval, in milliseconds
 *
 * @private
 */
RetryHandler.prototype.nextInterval_ = function () {
    var interval = this.interval * 2 + this.getRandomInt_(0, 1000);
    return Math.min(interval, this.maxInterval);
};

/**
 * Get a random int in the range of min to max. Used to add jitter to wait times.
 *
 * @param {number} min Lower bounds
 * @param {number} max Upper bounds
 * @private
 */
RetryHandler.prototype.getRandomInt_ = function (min, max) {
    return Math.floor(Math.random() * (max - min + 1) + min);
};


/**
 * Helper class for resumable uploads using XHR/CORS. Can upload any Blob-like item, whether
 * files or in-memory constructs.
 *
 * @example
 * var content = new Blob(["Hello world"], {"type": "text/plain"});
 * var uploader = new MediaUploader({
 *   file: content,
 *   token: accessToken,
 *   onComplete: function(data) { ... }
 *   onError: function(data) { ... }
 * });
 * uploader.upload();
 *
 * @constructor
 * @param {object} options Hash of options
 * @param {string} options.token Access token
 * @param {blob} options.file Blob-like item to upload
 * @param {string} [options.fileId] ID of file if replacing
 * @param {object} [options.params] Additional query parameters
 * @param {string} [options.contentType] Content-type, if overriding the type of the blob.
 * @param {object} [options.metadata] File metadata
 * @param {function} [options.onComplete] Callback for when upload is complete
 * @param {function} [options.onProgress] Callback for status for the in-progress upload
 * @param {function} [options.onError] Callback if upload fails
 */
var MediaUploader = function (options) {
    var noop = function () { };
    this.file = options.file;
    this.contentType = options.contentType || this.file.type || 'application/octet-stream';
    this.metadata = options.metadata || {
        'title': this.file.name,
        'TipoMimeType': this.contentType
    };
    this.token = options.token;
    this.onComplete = options.onComplete || noop;
    this.onProgress = options.onProgress || noop;
    this.onError = options.onError || noop;
    this.offset = options.offset || 0;
    this.chunkSize = options.chunkSize || 0;
    this.retryHandler = new RetryHandler();

    this.url = options.url;
    if (!this.url) {
        var params = options.params || {};
        params.uploadType = 'resumable';
        this.url = this.buildUrl_(options.fileId, params, options.baseUrl);
    }
    this.httpMethod = options.fileId ? 'PUT' : 'POST';
};

/**
 * Initiate the upload.
 */
MediaUploader.prototype.upload = function () {
    var self = this;
    var xhr = new XMLHttpRequest();

    xhr.open(this.httpMethod, this.url, true);
    xhr.setRequestHeader('Authorization', 'Bearer ' + this.token);
    xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.setRequestHeader('X-Upload-Content-Length', this.file.size);
    xhr.setRequestHeader('X-Upload-Content-Type', this.contentType);

    xhr.onload = function (e) {
        if (e.target.status < 400) {
            var location = e.target.getResponseHeader('Location');
            this.url = location;
            this.sendFile_();
        } else {
            this.onUploadError_(e);
        }
    }.bind(this);
    xhr.onerror = this.onUploadError_.bind(this);
    xhr.send(JSON.stringify(this.metadata));
};

/**
 * Send the actual file content.
 *
 * @private
 */
MediaUploader.prototype.sendFile_ = function () {
    var content = this.file;
    var end = this.file.size;

    if (this.offset || this.chunkSize) {
        // Only bother to slice the file if we're either resuming or uploading in chunks
        if (this.chunkSize) {
            end = Math.min(this.offset + this.chunkSize, this.file.size);
        }
        content = content.slice(this.offset, end);
    }

    var xhr = new XMLHttpRequest();
    xhr.open('PUT', this.url, true);
    xhr.setRequestHeader('Content-Type', this.contentType);
    xhr.setRequestHeader('Content-Range', 'bytes ' + this.offset + '-' + (end - 1) + '/' + this.file.size);
    xhr.setRequestHeader('X-Upload-Content-Type', this.file.type);
    if (xhr.upload) {
        xhr.upload.addEventListener('progress', this.onProgress);
    }
    xhr.onload = this.onContentUploadSuccess_.bind(this);
    xhr.onerror = this.onContentUploadError_.bind(this);
    xhr.send(content);
};

/**
 * Query for the state of the file for resumption.
 *
 * @private
 */
MediaUploader.prototype.resume_ = function () {
    var xhr = new XMLHttpRequest();
    xhr.open('PUT', this.url, true);
    xhr.setRequestHeader('Content-Range', 'bytes */' + this.file.size);
    xhr.setRequestHeader('X-Upload-Content-Type', this.file.type);
    if (xhr.upload) {
        xhr.upload.addEventListener('progress', this.onProgress);
    }
    xhr.onload = this.onContentUploadSuccess_.bind(this);
    xhr.onerror = this.onContentUploadError_.bind(this);
    xhr.send();
};

/**
 * Extract the last saved range if available in the request.
 *
 * @param {XMLHttpRequest} xhr Request object
 */
MediaUploader.prototype.extractRange_ = function (xhr) {
    var range = xhr.getResponseHeader('Range');
    if (range) {
        this.offset = parseInt(range.match(/\d+/g).pop(), 10) + 1;
    }
};

/**
 * Handle successful responses for uploads. Depending on the context,
 * may continue with uploading the next chunk of the file or, if complete,
 * invokes the caller's callback.
 *
 * @private
 * @param {object} e XHR event
 */
MediaUploader.prototype.onContentUploadSuccess_ = function (e) {
    if (e.target.status == 200 || e.target.status == 201) {
        this.onComplete(e.target.response);
    } else if (e.target.status == 308) {
        this.extractRange_(e.target);
        this.retryHandler.reset();
        this.sendFile_();
    }
};

/**
 * Handles errors for uploads. Either retries or aborts depending
 * on the error.
 *
 * @private
 * @param {object} e XHR event
 */
MediaUploader.prototype.onContentUploadError_ = function (e) {
    if (e.target.status && e.target.status < 500) {
        this.onError(e.target.response);
    } else {
        this.retryHandler.retry(this.resume_.bind(this));
    }
};

/**
 * Handles errors for the initial request.
 *
 * @private
 * @param {object} e XHR event
 */
MediaUploader.prototype.onUploadError_ = function (e) {
    this.onError(e.target.response); // TODO - Retries for initial upload
};

/**
 * Construct a query string from a hash/object
 *
 * @private
 * @param {object} [params] Key/value pairs for query string
 * @return {string} query string
 */
MediaUploader.prototype.buildQuery_ = function (params) {
    params = params || {};
    return Object.keys(params).map(function (key) {
        return encodeURIComponent(key) + '=' + encodeURIComponent(params[key]);
    }).join('&');
};

/**
 * Build the drive upload URL
 *
 * @private
 * @param {string} [id] File ID if replacing
 * @param {object} [params] Query parameters
 * @return {string} URL
 */
MediaUploader.prototype.buildUrl_ = function (id, params, baseUrl) {
    var url = baseUrl || DRIVE_UPLOAD_URL;
    if (id) {
        url += id;
    }
    var query = this.buildQuery_(params);
    if (query) {
        url += '?' + query;
    }
    return url;
};



//
// TipoMimeType.js - A catalog object of mime types based on file extensions
//
// @author: R. S. Doiel, <rsdoiel@gmail.com>
// copyright (c) 2012 all rights reserved
//
// Released under New the BSD License.
// See: http://opensource.org/licenses/bsd-license.php
//
/*jslint indent: 4 */
/*global require, exports */
(function (self) {
    "use strict";
    var path, TipoMimeType;

    // If we're NodeJS I can use the path module.
    // If I'm MongoDB shell, not available.
  
        path = {
            extname: function (filename) {
                if (filename.lastIndexOf(".") > 0) {
                    return filename.substr(filename.lastIndexOf("."));
                }
            }
        };
    

    TipoMimeType = {
        charset: 'UTF-8',
        catalog: {},
        lookup: function (fname, include_charset, default_mime_type) {
            var ext, charset = this.charset;

            if (include_charset === undefined) {
                include_charset = false;
            }

            if (typeof include_charset === "string") {
                charset = include_charset;
                include_charset = true;
            }

            if (path.extname !== undefined) {
                ext = path.extname(fname).toLowerCase();
            } else if (fname.lastIndexOf('.') > 0) {
                ext = fname.substr(fname.lastIndexOf('.')).toLowerCase();
            } else {
                ext = fname;
            }

            // Handle the special cases where their is no extension
            // e..g README, manifest, LICENSE, TODO
            if (ext === "") {
                ext = fname;
            }

            if (this.catalog[ext] !== undefined) {
                if (include_charset === true &&
                        this.catalog[ext].indexOf('text/') === 0 &&
                        this.catalog[ext].indexOf('charset') < 0) {
                    return this.catalog[ext] + '; charset=' + charset;
                } else {
                    return this.catalog[ext];
                }
            } else if (default_mime_type !== undefined) {
                if (include_charset === true &&
                        default_mime_type.indexOf('text/') === 0) {
                    return default_mime_type + '; charset=' + charset;
                }
                return default_mime_type;
            }
            return false;
        },
        set: function (exts, mime_type_string) {
            var result = true, self = this;
            //console.log("DEBUG exts.indexOf(',')", typeof exts.indexOf(','), exts.indexOf(','));
            if (exts.indexOf(',') > -1) {
                exts.split(',').forEach(function (ext) {
                    ext = ext.trim();
                    self.catalog[ext] = mime_type_string;
                    if (self.catalog[ext] !== mime_type_string) {
                        result = false;
                    }
                });
            } else {
                self.catalog[exts] = mime_type_string;
            }
            return result;
        },
        del: function (ext) {
            delete this.catalog[ext];
            return (this.catalog[ext] === undefined);
        },
        forEach: function (callback) {
            var self = this, ext;
            // Mongo 2.2. Shell doesn't support Object.keys()
            for (ext in self.catalog) {
                if (self.catalog.hasOwnProperty(ext)) {
                    callback(ext, self.catalog[ext]);
                }
            }
            return self.catalog;
        }
    };

    // From Apache project's mime type list.
    TipoMimeType.set(".ez", "application/andrew-inset");
    TipoMimeType.set(".aw", "application/applixware");
    TipoMimeType.set(".atom", "application/atom+xml");
    TipoMimeType.set(".atomcat", "application/atomcat+xml");
    TipoMimeType.set(".atomsvc", "application/atomsvc+xml");
    TipoMimeType.set(".ccxml", "application/ccxml+xml");
    TipoMimeType.set(".cu", "application/cu-seeme");
    TipoMimeType.set(".davmount", "application/davmount+xml");
    TipoMimeType.set(".ecma", "application/ecmascript");
    TipoMimeType.set(".emma", "application/emma+xml");
    TipoMimeType.set(".epub", "application/epub+zip");
    TipoMimeType.set(".pfr", "application/font-tdpfr");
    TipoMimeType.set(".stk", "application/hyperstudio");
    TipoMimeType.set(".jar", "application/java-archive");
    TipoMimeType.set(".ser", "application/java-serialized-object");
    TipoMimeType.set(".class", "application/java-vm");
    TipoMimeType.set(".js", "application/javascript");
    TipoMimeType.set(".json", "application/json");
    TipoMimeType.set(".lostxml", "application/lost+xml");
    TipoMimeType.set(".hqx", "application/mac-binhex40");
    TipoMimeType.set(".cpt", "application/mac-compactpro");
    TipoMimeType.set(".mrc", "application/marc");
    TipoMimeType.set(".ma,.nb,.mb", "application/mathematica");
    TipoMimeType.set(".mathml", "application/mathml+xml");
    TipoMimeType.set(".mbox", "application/mbox");
    TipoMimeType.set(".mscml", "application/mediaservercontrol+xml");
    TipoMimeType.set(".mp4s", "application/mp4");
    TipoMimeType.set(".doc,.dot", "application/msword");
    TipoMimeType.set(".mxf", "application/mxf");
    TipoMimeType.set(".oda", "application/oda");
    TipoMimeType.set(".opf", "application/oebps-package+xml");
    TipoMimeType.set(".ogx", "application/ogg");
    TipoMimeType.set(".onetoc,.onetoc2,.onetmp,.onepkg", "application/onenote");
    TipoMimeType.set(".xer", "application/patch-ops-error+xml");
    TipoMimeType.set(".pdf", "application/pdf");
    TipoMimeType.set(".pgp", "application/pgp-encrypted");
    TipoMimeType.set(".asc,.sig", "application/pgp-signature");
    TipoMimeType.set(".prf", "application/pics-rules");
    TipoMimeType.set(".p10", "application/pkcs10");
    TipoMimeType.set(".p7m,.p7c", "application/pkcs7-mime");
    TipoMimeType.set(".p7s", "application/pkcs7-signature");
    TipoMimeType.set(".cer", "application/pkix-cert");
    TipoMimeType.set(".crl", "application/pkix-crl");
    TipoMimeType.set(".pkipath", "application/pkix-pkipath");
    TipoMimeType.set(".pki", "application/pkixcmp");
    TipoMimeType.set(".pls", "application/pls+xml");
    TipoMimeType.set(".ai,.eps,.ps", "application/postscript");
    TipoMimeType.set(".cww", "application/prs.cww");
    TipoMimeType.set(".rdf", "application/rdf+xml");
    TipoMimeType.set(".rif", "application/reginfo+xml");
    TipoMimeType.set(".rnc", "application/relax-ng-compact-syntax");
    TipoMimeType.set(".rl", "application/resource-lists+xml");
    TipoMimeType.set(".rld", "application/resource-lists-diff+xml");
    TipoMimeType.set(".rs", "application/rls-services+xml");
    TipoMimeType.set(".rsd", "application/rsd+xml");
    TipoMimeType.set(".rss", "application/rss+xml");
    TipoMimeType.set(".rtf", "application/rtf");
    TipoMimeType.set(".sbml", "application/sbml+xml");
    TipoMimeType.set(".scq", "application/scvp-cv-request");
    TipoMimeType.set(".scs", "application/scvp-cv-response");
    TipoMimeType.set(".spq", "application/scvp-vp-request");
    TipoMimeType.set(".spp", "application/scvp-vp-response");
    TipoMimeType.set(".sdp", "application/sdp");
    TipoMimeType.set(".setpay", "application/set-payment-initiation");
    TipoMimeType.set(".setreg", "application/set-registration-initiation");
    TipoMimeType.set(".shf", "application/shf+xml");
    TipoMimeType.set(".smi,.smil", "application/smil+xml");
    TipoMimeType.set(".rq", "application/sparql-query");
    TipoMimeType.set(".srx", "application/sparql-results+xml");
    TipoMimeType.set(".gram", "application/srgs");
    TipoMimeType.set(".grxml", "application/srgs+xml");
    TipoMimeType.set(".ssml", "application/ssml+xml");
    TipoMimeType.set(".plb", "application/vnd.3gpp.pic-bw-large");
    TipoMimeType.set(".psb", "application/vnd.3gpp.pic-bw-small");
    TipoMimeType.set(".pvb", "application/vnd.3gpp.pic-bw-var");
    TipoMimeType.set(".tcap", "application/vnd.3gpp2.tcap");
    TipoMimeType.set(".pwn", "application/vnd.3m.post-it-notes");
    TipoMimeType.set(".aso", "application/vnd.accpac.simply.aso");
    TipoMimeType.set(".imp", "application/vnd.accpac.simply.imp");
    TipoMimeType.set(".acu", "application/vnd.acucobol");
    TipoMimeType.set(".atc,.acutc", "application/vnd.acucorp");
    TipoMimeType.set(".air", "application/vnd.adobe.air-application-installer-package+zip");
    TipoMimeType.set(".xdp", "application/vnd.adobe.xdp+xml");
    TipoMimeType.set(".xfdf", "application/vnd.adobe.xfdf");
    TipoMimeType.set(".azf", "application/vnd.airzip.filesecure.azf");
    TipoMimeType.set(".azs", "application/vnd.airzip.filesecure.azs");
    TipoMimeType.set(".azw", "application/vnd.amazon.ebook");
    TipoMimeType.set(".acc", "application/vnd.americandynamics.acc");
    TipoMimeType.set(".ami", "application/vnd.amiga.ami");
    TipoMimeType.set(".apk", "application/vnd.android.package-archive");
    TipoMimeType.set(".cii", "application/vnd.anser-web-certificate-issue-initiation");
    TipoMimeType.set(".fti", "application/vnd.anser-web-funds-transfer-initiation");
    TipoMimeType.set(".atx", "application/vnd.antix.game-component");
    TipoMimeType.set(".mpkg", "application/vnd.apple.installer+xml");
    TipoMimeType.set(".swi", "application/vnd.arastra.swi");
    TipoMimeType.set(".aep", "application/vnd.audiograph");
    TipoMimeType.set(".mpm", "application/vnd.blueice.multipass");
    TipoMimeType.set(".bmi", "application/vnd.bmi");
    TipoMimeType.set(".rep", "application/vnd.businessobjects");
    TipoMimeType.set(".cdxml", "application/vnd.chemdraw+xml");
    TipoMimeType.set(".mmd", "application/vnd.chipnuts.karaoke-mmd");
    TipoMimeType.set(".cdy", "application/vnd.cinderella");
    TipoMimeType.set(".cla", "application/vnd.claymore");
    TipoMimeType.set(".c4g,.c4d,.c4f,.c4p,.c4u", "application/vnd.clonk.c4group");
    TipoMimeType.set(".csp", "application/vnd.commonspace");
    TipoMimeType.set(".cdbcmsg", "application/vnd.contact.cmsg");
    TipoMimeType.set(".cmc", "application/vnd.cosmocaller");
    TipoMimeType.set(".clkx", "application/vnd.crick.clicker");
    TipoMimeType.set(".clkk", "application/vnd.crick.clicker.keyboard");
    TipoMimeType.set(".clkp", "application/vnd.crick.clicker.palette");
    TipoMimeType.set(".clkt", "application/vnd.crick.clicker.template");
    TipoMimeType.set(".clkw", "application/vnd.crick.clicker.wordbank");
    TipoMimeType.set(".wbs", "application/vnd.criticaltools.wbs+xml");
    TipoMimeType.set(".pml", "application/vnd.ctc-posml");
    TipoMimeType.set(".ppd", "application/vnd.cups-ppd");
    TipoMimeType.set(".car", "application/vnd.curl.car");
    TipoMimeType.set(".pcurl", "application/vnd.curl.pcurl");
    TipoMimeType.set(".rdz", "application/vnd.data-vision.rdz");
    TipoMimeType.set(".fe_launch", "application/vnd.denovo.fcselayout-link");
    TipoMimeType.set(".dna", "application/vnd.dna");
    TipoMimeType.set(".mlp", "application/vnd.dolby.mlp");
    TipoMimeType.set(".dpg", "application/vnd.dpgraph");
    TipoMimeType.set(".dfac", "application/vnd.dreamfactory");
    TipoMimeType.set(".geo", "application/vnd.dynageo");
    TipoMimeType.set(".mag", "application/vnd.ecowin.chart");
    TipoMimeType.set(".nml", "application/vnd.enliven");
    TipoMimeType.set(".esf", "application/vnd.epson.esf");
    TipoMimeType.set(".msf", "application/vnd.epson.msf");
    TipoMimeType.set(".qam", "application/vnd.epson.quickanime");
    TipoMimeType.set(".slt", "application/vnd.epson.salt");
    TipoMimeType.set(".ssf", "application/vnd.epson.ssf");
    TipoMimeType.set(".es3,.et3", "application/vnd.eszigno3+xml");
    TipoMimeType.set(".ez2", "application/vnd.ezpix-album");
    TipoMimeType.set(".ez3", "application/vnd.ezpix-package");
    TipoMimeType.set(".fdf", "application/vnd.fdf");
    TipoMimeType.set(".mseed", "application/vnd.fdsn.mseed");
    TipoMimeType.set(".seed,.dataless", "application/vnd.fdsn.seed");
    TipoMimeType.set(".gph", "application/vnd.flographit");
    TipoMimeType.set(".ftc", "application/vnd.fluxtime.clip");
    TipoMimeType.set(".fm,.frame,.maker,.book", "application/vnd.framemaker");
    TipoMimeType.set(".fnc", "application/vnd.frogans.fnc");
    TipoMimeType.set(".ltf", "application/vnd.frogans.ltf");
    TipoMimeType.set(".fsc", "application/vnd.fsc.weblaunch");
    TipoMimeType.set(".oas", "application/vnd.fujitsu.oasys");
    TipoMimeType.set(".oa2", "application/vnd.fujitsu.oasys2");
    TipoMimeType.set(".oa3", "application/vnd.fujitsu.oasys3");
    TipoMimeType.set(".fg5", "application/vnd.fujitsu.oasysgp");
    TipoMimeType.set(".bh2", "application/vnd.fujitsu.oasysprs");
    TipoMimeType.set(".ddd", "application/vnd.fujixerox.ddd");
    TipoMimeType.set(".xdw", "application/vnd.fujixerox.docuworks");
    TipoMimeType.set(".xbd", "application/vnd.fujixerox.docuworks.binder");
    TipoMimeType.set(".fzs", "application/vnd.fuzzysheet");
    TipoMimeType.set(".txd", "application/vnd.genomatix.tuxedo");
    TipoMimeType.set(".ggb", "application/vnd.geogebra.file");
    TipoMimeType.set(".ggt", "application/vnd.geogebra.tool");
    TipoMimeType.set(".gex,.gre", "application/vnd.geometry-explorer");
    TipoMimeType.set(".gmx", "application/vnd.gmx");
    TipoMimeType.set(".kml", "application/vnd.google-earth.kml+xml");
    TipoMimeType.set(".kmz", "application/vnd.google-earth.kmz");
    TipoMimeType.set(".gqf,.gqs", "application/vnd.grafeq");
    TipoMimeType.set(".gac", "application/vnd.groove-account");
    TipoMimeType.set(".ghf", "application/vnd.groove-help");
    TipoMimeType.set(".gim", "application/vnd.groove-identity-message");
    TipoMimeType.set(".grv", "application/vnd.groove-injector");
    TipoMimeType.set(".gtm", "application/vnd.groove-tool-message");
    TipoMimeType.set(".tpl", "application/vnd.groove-tool-template");
    TipoMimeType.set(".vcg", "application/vnd.groove-vcard");
    TipoMimeType.set(".zmm", "application/vnd.handheld-entertainment+xml");
    TipoMimeType.set(".hbci", "application/vnd.hbci");
    TipoMimeType.set(".les", "application/vnd.hhe.lesson-player");
    TipoMimeType.set(".hpgl", "application/vnd.hp-hpgl");
    TipoMimeType.set(".hpid", "application/vnd.hp-hpid");
    TipoMimeType.set(".hps", "application/vnd.hp-hps");
    TipoMimeType.set(".jlt", "application/vnd.hp-jlyt");
    TipoMimeType.set(".pcl", "application/vnd.hp-pcl");
    TipoMimeType.set(".pclxl", "application/vnd.hp-pclxl");
    TipoMimeType.set(".sfd-hdstx", "application/vnd.hydrostatix.sof-data");
    TipoMimeType.set(".x3d", "application/vnd.hzn-3d-crossword");
    TipoMimeType.set(".mpy", "application/vnd.ibm.minipay");
    TipoMimeType.set(".afp,.listafp,.list3820", "application/vnd.ibm.modcap");
    TipoMimeType.set(".irm", "application/vnd.ibm.rights-management");
    TipoMimeType.set(".sc", "application/vnd.ibm.secure-container");
    TipoMimeType.set(".icc,.icm", "application/vnd.iccprofile");
    TipoMimeType.set(".igl", "application/vnd.igloader");
    TipoMimeType.set(".ivp", "application/vnd.immervision-ivp");
    TipoMimeType.set(".ivu", "application/vnd.immervision-ivu");
    TipoMimeType.set(".xpw,.xpx", "application/vnd.intercon.formnet");
    TipoMimeType.set(".qbo", "application/vnd.intu.qbo");
    TipoMimeType.set(".qfx", "application/vnd.intu.qfx");
    TipoMimeType.set(".rcprofile", "application/vnd.ipunplugged.rcprofile");
    TipoMimeType.set(".irp", "application/vnd.irepository.package+xml");
    TipoMimeType.set(".xpr", "application/vnd.is-xpr");
    TipoMimeType.set(".jam", "application/vnd.jam");
    TipoMimeType.set(".rms", "application/vnd.jcp.javame.midlet-rms");
    TipoMimeType.set(".jisp", "application/vnd.jisp");
    TipoMimeType.set(".joda", "application/vnd.joost.joda-archive");
    TipoMimeType.set(".ktz,.ktr", "application/vnd.kahootz");
    TipoMimeType.set(".karbon", "application/vnd.kde.karbon");
    TipoMimeType.set(".chrt", "application/vnd.kde.kchart");
    TipoMimeType.set(".kfo", "application/vnd.kde.kformula");
    TipoMimeType.set(".flw", "application/vnd.kde.kivio");
    TipoMimeType.set(".kon", "application/vnd.kde.kontour");
    TipoMimeType.set(".kpr,.kpt", "application/vnd.kde.kpresenter");
    TipoMimeType.set(".ksp", "application/vnd.kde.kspread");
    TipoMimeType.set(".kwd,.kwt", "application/vnd.kde.kword");
    TipoMimeType.set(".htke", "application/vnd.kenameaapp");
    TipoMimeType.set(".kia", "application/vnd.kidspiration");
    TipoMimeType.set(".kne,.knp", "application/vnd.kinar");
    TipoMimeType.set(".skp,.skd,.skt,.skm", "application/vnd.koan");
    TipoMimeType.set(".sse", "application/vnd.kodak-descriptor");
    TipoMimeType.set(".lbd", "application/vnd.llamagraphics.life-balance.desktop");
    TipoMimeType.set(".lbe", "application/vnd.llamagraphics.life-balance.exchange+xml");
    TipoMimeType.set(".123", "application/vnd.lotus-1-2-3");
    TipoMimeType.set(".apr", "application/vnd.lotus-approach");
    TipoMimeType.set(".pre", "application/vnd.lotus-freelance");
    TipoMimeType.set(".nsf", "application/vnd.lotus-notes");
    TipoMimeType.set(".org", "application/vnd.lotus-organizer");
    TipoMimeType.set(".scm", "application/vnd.lotus-screencam");
    TipoMimeType.set(".lwp", "application/vnd.lotus-wordpro");
    TipoMimeType.set(".portpkg", "application/vnd.macports.portpkg");
    TipoMimeType.set(".mcd", "application/vnd.mcd");
    TipoMimeType.set(".mc1", "application/vnd.medcalcdata");
    TipoMimeType.set(".cdkey", "application/vnd.mediastation.cdkey");
    TipoMimeType.set(".mwf", "application/vnd.mfer");
    TipoMimeType.set(".mfm", "application/vnd.mfmp");
    TipoMimeType.set(".flo", "application/vnd.micrografx.flo");
    TipoMimeType.set(".igx", "application/vnd.micrografx.igx");
    TipoMimeType.set(".mif", "application/vnd.mif");
    TipoMimeType.set(".daf", "application/vnd.mobius.daf");
    TipoMimeType.set(".dis", "application/vnd.mobius.dis");
    TipoMimeType.set(".mbk", "application/vnd.mobius.mbk");
    TipoMimeType.set(".mqy", "application/vnd.mobius.mqy");
    TipoMimeType.set(".msl", "application/vnd.mobius.msl");
    TipoMimeType.set(".plc", "application/vnd.mobius.plc");
    TipoMimeType.set(".txf", "application/vnd.mobius.txf");
    TipoMimeType.set(".mpn", "application/vnd.mophun.application");
    TipoMimeType.set(".mpc", "application/vnd.mophun.certificate");
    TipoMimeType.set(".xul", "application/vnd.mozilla.xul+xml");
    TipoMimeType.set(".cil", "application/vnd.ms-artgalry");
    TipoMimeType.set(".cab", "application/vnd.ms-cab-compressed");
    TipoMimeType.set(".xls,.xlm,.xla,.xlc,.xlt,.xlw", "application/vnd.ms-excel");
    TipoMimeType.set(".xlam", "application/vnd.ms-excel.addin.macroenabled.12");
    TipoMimeType.set(".xlsb", "application/vnd.ms-excel.sheet.binary.macroenabled.12");
    TipoMimeType.set(".xlsm", "application/vnd.ms-excel.sheet.macroenabled.12");
    TipoMimeType.set(".xltm", "application/vnd.ms-excel.template.macroenabled.12");
    TipoMimeType.set(".eot", "application/vnd.ms-fontobject");
    TipoMimeType.set(".chm", "application/vnd.ms-htmlhelp");
    TipoMimeType.set(".ims", "application/vnd.ms-ims");
    TipoMimeType.set(".lrm", "application/vnd.ms-lrm");
    TipoMimeType.set(".cat", "application/vnd.ms-pki.seccat");
    TipoMimeType.set(".stl", "application/vnd.ms-pki.stl");
    TipoMimeType.set(".ppt,.pps,.pot", "application/vnd.ms-powerpoint");
    TipoMimeType.set(".ppam", "application/vnd.ms-powerpoint.addin.macroenabled.12");
    TipoMimeType.set(".pptm", "application/vnd.ms-powerpoint.presentation.macroenabled.12");
    TipoMimeType.set(".sldm", "application/vnd.ms-powerpoint.slide.macroenabled.12");
    TipoMimeType.set(".ppsm", "application/vnd.ms-powerpoint.slideshow.macroenabled.12");
    TipoMimeType.set(".potm", "application/vnd.ms-powerpoint.template.macroenabled.12");
    TipoMimeType.set(".mpp,.mpt", "application/vnd.ms-project");
    TipoMimeType.set(".docm", "application/vnd.ms-word.document.macroenabled.12");
    TipoMimeType.set(".dotm", "application/vnd.ms-word.template.macroenabled.12");
    TipoMimeType.set(".wps,.wks,.wcm,.wdb", "application/vnd.ms-works");
    TipoMimeType.set(".wpl", "application/vnd.ms-wpl");
    TipoMimeType.set(".xps", "application/vnd.ms-xpsdocument");
    TipoMimeType.set(".mseq", "application/vnd.mseq");
    TipoMimeType.set(".mus", "application/vnd.musician");
    TipoMimeType.set(".msty", "application/vnd.muvee.style");
    TipoMimeType.set(".nlu", "application/vnd.neurolanguage.nlu");
    TipoMimeType.set(".nnd", "application/vnd.noblenet-directory");
    TipoMimeType.set(".nns", "application/vnd.noblenet-sealer");
    TipoMimeType.set(".nnw", "application/vnd.noblenet-web");
    TipoMimeType.set(".ngdat", "application/vnd.nokia.n-gage.data");
    TipoMimeType.set(".n-gage", "application/vnd.nokia.n-gage.symbian.install");
    TipoMimeType.set(".rpst", "application/vnd.nokia.radio-preset");
    TipoMimeType.set(".rpss", "application/vnd.nokia.radio-presets");
    TipoMimeType.set(".edm", "application/vnd.novadigm.edm");
    TipoMimeType.set(".edx", "application/vnd.novadigm.edx");
    TipoMimeType.set(".ext", "application/vnd.novadigm.ext");
    TipoMimeType.set(".odc", "application/vnd.oasis.opendocument.chart");
    TipoMimeType.set(".otc", "application/vnd.oasis.opendocument.chart-template");
    TipoMimeType.set(".odb", "application/vnd.oasis.opendocument.database");
    TipoMimeType.set(".odf", "application/vnd.oasis.opendocument.formula");
    TipoMimeType.set(".odft", "application/vnd.oasis.opendocument.formula-template");
    TipoMimeType.set(".odg", "application/vnd.oasis.opendocument.graphics");
    TipoMimeType.set(".otg", "application/vnd.oasis.opendocument.graphics-template");
    TipoMimeType.set(".odi", "application/vnd.oasis.opendocument.image");
    TipoMimeType.set(".oti", "application/vnd.oasis.opendocument.image-template");
    TipoMimeType.set(".odp", "application/vnd.oasis.opendocument.presentation");
    TipoMimeType.set(".ods", "application/vnd.oasis.opendocument.spreadsheet");
    TipoMimeType.set(".ots", "application/vnd.oasis.opendocument.spreadsheet-template");
    TipoMimeType.set(".odt", "application/vnd.oasis.opendocument.text");
    TipoMimeType.set(".otm", "application/vnd.oasis.opendocument.text-master");
    TipoMimeType.set(".ott", "application/vnd.oasis.opendocument.text-template");
    TipoMimeType.set(".oth", "application/vnd.oasis.opendocument.text-web");
    TipoMimeType.set(".xo", "application/vnd.olpc-sugar");
    TipoMimeType.set(".dd2", "application/vnd.oma.dd2+xml");
    TipoMimeType.set(".oxt", "application/vnd.openofficeorg.extension");
    TipoMimeType.set(".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation");
    TipoMimeType.set(".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide");
    TipoMimeType.set(".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow");
    TipoMimeType.set(".potx", "application/vnd.openxmlformats-officedocument.presentationml.template");
    TipoMimeType.set(".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    TipoMimeType.set(".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template");
    TipoMimeType.set(".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
    TipoMimeType.set(".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template");
    TipoMimeType.set(".dp", "application/vnd.osgi.dp");
    TipoMimeType.set(".pdb,.pqa,.oprc", "application/vnd.palm");
    TipoMimeType.set(".str", "application/vnd.pg.format");
    TipoMimeType.set(".ei6", "application/vnd.pg.osasli");
    TipoMimeType.set(".efif", "application/vnd.picsel");
    TipoMimeType.set(".plf", "application/vnd.pocketlearn");
    TipoMimeType.set(".pbd", "application/vnd.powerbuilder6");
    TipoMimeType.set(".box", "application/vnd.previewsystems.box");
    TipoMimeType.set(".mgz", "application/vnd.proteus.magazine");
    TipoMimeType.set(".qps", "application/vnd.publishare-delta-tree");
    TipoMimeType.set(".ptid", "application/vnd.pvi.ptid1");
    TipoMimeType.set(".qxd,.qxt,.qwd,.qwt,.qxl,.qxb", "application/vnd.quark.quarkxpress");
    TipoMimeType.set(".mxl", "application/vnd.recordare.musicxml");
    TipoMimeType.set(".musicxml", "application/vnd.recordare.musicxml+xml");
    TipoMimeType.set(".cod", "application/vnd.rim.cod");
    TipoMimeType.set(".rm", "application/vnd.rn-realmedia");
    TipoMimeType.set(".link66", "application/vnd.route66.link66+xml");
    TipoMimeType.set(".see", "application/vnd.seemail");
    TipoMimeType.set(".sema", "application/vnd.sema");
    TipoMimeType.set(".semd", "application/vnd.semd");
    TipoMimeType.set(".semf", "application/vnd.semf");
    TipoMimeType.set(".ifm", "application/vnd.shana.informed.formdata");
    TipoMimeType.set(".itp", "application/vnd.shana.informed.formtemplate");
    TipoMimeType.set(".iif", "application/vnd.shana.informed.interchange");
    TipoMimeType.set(".ipk", "application/vnd.shana.informed.package");
    TipoMimeType.set(".twd,.twds", "application/vnd.simtech-mindmapper");
    TipoMimeType.set(".mmf", "application/vnd.smaf");
    TipoMimeType.set(".teacher", "application/vnd.smart.teacher");
    TipoMimeType.set(".sdkm,.sdkd", "application/vnd.solent.sdkm+xml");
    TipoMimeType.set(".dxp", "application/vnd.spotfire.dxp");
    TipoMimeType.set(".sfs", "application/vnd.spotfire.sfs");
    TipoMimeType.set(".sdc", "application/vnd.stardivision.calc");
    TipoMimeType.set(".sda", "application/vnd.stardivision.draw");
    TipoMimeType.set(".sdd", "application/vnd.stardivision.impress");
    TipoMimeType.set(".smf", "application/vnd.stardivision.math");
    TipoMimeType.set(".sdw", "application/vnd.stardivision.writer");
    TipoMimeType.set(".vor", "application/vnd.stardivision.writer");
    TipoMimeType.set(".sgl", "application/vnd.stardivision.writer-global");
    TipoMimeType.set(".sxc", "application/vnd.sun.xml.calc");
    TipoMimeType.set(".stc", "application/vnd.sun.xml.calc.template");
    TipoMimeType.set(".sxd", "application/vnd.sun.xml.draw");
    TipoMimeType.set(".std", "application/vnd.sun.xml.draw.template");
    TipoMimeType.set(".sxi", "application/vnd.sun.xml.impress");
    TipoMimeType.set(".sti", "application/vnd.sun.xml.impress.template");
    TipoMimeType.set(".sxm", "application/vnd.sun.xml.math");
    TipoMimeType.set(".sxw", "application/vnd.sun.xml.writer");
    TipoMimeType.set(".sxg", "application/vnd.sun.xml.writer.global");
    TipoMimeType.set(".stw", "application/vnd.sun.xml.writer.template");
    TipoMimeType.set(".sus,.susp", "application/vnd.sus-calendar");
    TipoMimeType.set(".svd", "application/vnd.svd");
    TipoMimeType.set(".sis,.sisx", "application/vnd.symbian.install");
    TipoMimeType.set(".xsm", "application/vnd.syncml+xml");
    TipoMimeType.set(".bdm", "application/vnd.syncml.dm+wbxml");
    TipoMimeType.set(".xdm", "application/vnd.syncml.dm+xml");
    TipoMimeType.set(".tao", "application/vnd.tao.intent-module-archive");
    TipoMimeType.set(".tmo", "application/vnd.tmobile-livetv");
    TipoMimeType.set(".tpt", "application/vnd.trid.tpt");
    TipoMimeType.set(".mxs", "application/vnd.triscape.mxs");
    TipoMimeType.set(".tra", "application/vnd.trueapp");
    TipoMimeType.set(".ufd,.ufdl", "application/vnd.ufdl");
    TipoMimeType.set(".utz", "application/vnd.uiq.theme");
    TipoMimeType.set(".umj", "application/vnd.umajin");
    TipoMimeType.set(".unityweb", "application/vnd.unity");
    TipoMimeType.set(".uoml", "application/vnd.uoml+xml");
    TipoMimeType.set(".vcx", "application/vnd.vcx");
    TipoMimeType.set(".vsd,.vst,.vss,.vsw", "application/vnd.visio");
    TipoMimeType.set(".vis", "application/vnd.visionary");
    TipoMimeType.set(".vsf", "application/vnd.vsf");
    TipoMimeType.set(".wbxml", "application/vnd.wap.wbxml");
    TipoMimeType.set(".wmlc", "application/vnd.wap.wmlc");
    TipoMimeType.set(".wmlsc", "application/vnd.wap.wmlscriptc");
    TipoMimeType.set(".wtb", "application/vnd.webturbo");
    TipoMimeType.set(".wpd", "application/vnd.wordperfect");
    TipoMimeType.set(".wqd", "application/vnd.wqd");
    TipoMimeType.set(".stf", "application/vnd.wt.stf");
    TipoMimeType.set(".xar", "application/vnd.xara");
    TipoMimeType.set(".xfdl", "application/vnd.xfdl");
    TipoMimeType.set(".hvd", "application/vnd.yamaha.hv-dic");
    TipoMimeType.set(".hvs", "application/vnd.yamaha.hv-script");
    TipoMimeType.set(".hvp", "application/vnd.yamaha.hv-voice");
    TipoMimeType.set(".osf", "application/vnd.yamaha.openscoreformat");
    TipoMimeType.set(".osfpvg", "application/vnd.yamaha.openscoreformat.osfpvg+xml");
    TipoMimeType.set(".saf", "application/vnd.yamaha.smaf-audio");
    TipoMimeType.set(".spf", "application/vnd.yamaha.smaf-phrase");
    TipoMimeType.set(".cmp", "application/vnd.yellowriver-custom-menu");
    TipoMimeType.set(".zir,.zirz", "application/vnd.zul");
    TipoMimeType.set(".zaz", "application/vnd.zzazz.deck+xml");
    TipoMimeType.set(".vxml", "application/voicexml+xml");
    TipoMimeType.set(".hlp", "application/winhlp");
    TipoMimeType.set(".wsdl", "application/wsdl+xml");
    TipoMimeType.set(".wspolicy", "application/wspolicy+xml");
    TipoMimeType.set(".abw", "application/x-abiword");
    TipoMimeType.set(".ace", "application/x-ace-compressed");
    TipoMimeType.set(".aab,.x32,.u32,.vox", "application/x-authorware-bin");
    TipoMimeType.set(".aam", "application/x-authorware-map");
    TipoMimeType.set(".aas", "application/x-authorware-seg");
    TipoMimeType.set(".bcpio", "application/x-bcpio");
    TipoMimeType.set(".torrent", "application/x-bittorrent");
    TipoMimeType.set(".bz", "application/x-bzip");
    TipoMimeType.set(".bz2,.boz", "application/x-bzip2");
    TipoMimeType.set(".vcd", "application/x-cdlink");
    TipoMimeType.set(".chat", "application/x-chat");
    TipoMimeType.set(".pgn", "application/x-chess-pgn");
    TipoMimeType.set(".cpio", "application/x-cpio");
    TipoMimeType.set(".csh", "application/x-csh");
    TipoMimeType.set(".deb,.udeb", "application/x-debian-package");
    TipoMimeType.set(".dir,.dcr,.dxr,.cst,.cct,.cxt,.w3d,.fgd,.swa", "application/x-director");
    TipoMimeType.set(".wad", "application/x-doom");
    TipoMimeType.set(".ncx", "application/x-dtbncx+xml");
    TipoMimeType.set(".dtb", "application/x-dtbook+xml");
    TipoMimeType.set(".res", "application/x-dtbresource+xml");
    TipoMimeType.set(".dvi", "application/x-dvi");
    TipoMimeType.set(".bdf", "application/x-font-bdf");
    TipoMimeType.set(".gsf", "application/x-font-ghostscript");
    TipoMimeType.set(".psf", "application/x-font-linux-psf");
    TipoMimeType.set(".otf", "application/x-font-otf");
    TipoMimeType.set(".pcf", "application/x-font-pcf");
    TipoMimeType.set(".snf", "application/x-font-snf");
    TipoMimeType.set(".ttf,.ttc", "application/x-font-ttf");
    TipoMimeType.set(".woff", "application/font-woff");
    TipoMimeType.set(".pfa,.pfb,.pfm,.afm", "application/x-font-type1");
    TipoMimeType.set(".spl", "application/x-futuresplash");
    TipoMimeType.set(".gnumeric", "application/x-gnumeric");
    TipoMimeType.set(".gtar", "application/x-gtar");
    TipoMimeType.set(".hdf", "application/x-hdf");
    TipoMimeType.set(".jnlp", "application/x-java-jnlp-file");
    TipoMimeType.set(".latex", "application/x-latex");
    TipoMimeType.set(".prc,.mobi", "application/x-mobipocket-ebook");
    TipoMimeType.set(".application", "application/x-ms-application");
    TipoMimeType.set(".wmd", "application/x-ms-wmd");
    TipoMimeType.set(".wmz", "application/x-ms-wmz");
    TipoMimeType.set(".xbap", "application/x-ms-xbap");
    TipoMimeType.set(".mdb", "application/x-msaccess");
    TipoMimeType.set(".obd", "application/x-msbinder");
    TipoMimeType.set(".crd", "application/x-mscardfile");
    TipoMimeType.set(".clp", "application/x-msclip");
    TipoMimeType.set(".exe,.dll,.com,.bat,.msi", "application/x-msdownload");
    TipoMimeType.set(".mvb,.m13,.m14", "application/x-msmediaview");
    TipoMimeType.set(".wmf", "application/x-msmetafile");
    TipoMimeType.set(".mny", "application/x-msmoney");
    TipoMimeType.set(".pub", "application/x-mspublisher");
    TipoMimeType.set(".scd", "application/x-msschedule");
    TipoMimeType.set(".trm", "application/x-msterminal");
    TipoMimeType.set(".wri", "application/x-mswrite");
    TipoMimeType.set(".nc,.cdf", "application/x-netcdf");
    TipoMimeType.set(".p12,.pfx", "application/x-pkcs12");
    TipoMimeType.set(".p7b,.spc", "application/x-pkcs7-certificates");
    TipoMimeType.set(".p7r", "application/x-pkcs7-certreqresp");
    TipoMimeType.set(".rar", "application/x-rar-compressed");
    TipoMimeType.set(".sh", "application/x-sh");
    TipoMimeType.set(".shar", "application/x-shar");
    TipoMimeType.set(".swf", "application/x-shockwave-flash");
    TipoMimeType.set(".xap", "application/x-silverlight-app");
    TipoMimeType.set(".sit", "application/x-stuffit");
    TipoMimeType.set(".sitx", "application/x-stuffitx");
    TipoMimeType.set(".sv4cpio", "application/x-sv4cpio");
    TipoMimeType.set(".sv4crc", "application/x-sv4crc");
    TipoMimeType.set(".tar", "application/x-tar");
    TipoMimeType.set(".tcl", "application/x-tcl");
    TipoMimeType.set(".tex", "application/x-tex");
    TipoMimeType.set(".tfm", "application/x-tex-tfm");
    TipoMimeType.set(".texinfo,.texi", "application/x-texinfo");
    TipoMimeType.set(".ustar", "application/x-ustar");
    TipoMimeType.set(".src", "application/x-wais-source");
    TipoMimeType.set(".der,.crt", "application/x-x509-ca-cert");
    TipoMimeType.set(".fig", "application/x-xfig");
    TipoMimeType.set(".xpi", "application/x-xpinstall");
    TipoMimeType.set(".xenc", "application/xenc+xml");
    TipoMimeType.set(".xhtml,.xht", "application/xhtml+xml");
    TipoMimeType.set(".xml,.xsl", "application/xml");
    TipoMimeType.set(".dtd", "application/xml-dtd");
    TipoMimeType.set(".xop", "application/xop+xml");
    TipoMimeType.set(".xslt", "application/xslt+xml");
    TipoMimeType.set(".xspf", "application/xspf+xml");
    TipoMimeType.set(".mxml,.xhvml,.xvml,.xvm", "application/xv+xml");
    TipoMimeType.set(".zip", "application/zip");
    TipoMimeType.set(".adp", "audio/adpcm");
    TipoMimeType.set(".au,.snd", "audio/basic");
    TipoMimeType.set(".mid,.midi,.kar,.rmi", "audio/midi");
    TipoMimeType.set(".mp4a", "audio/mp4");
    TipoMimeType.set(".m4a,.m4p", "audio/mp4a-latm");
    TipoMimeType.set(".mpga,.mp2,.mp2a,.mp3,.m2a,.m3a", "audio/mpeg");
    TipoMimeType.set(".oga,.ogg,.spx", "audio/ogg");
    TipoMimeType.set(".eol", "audio/vnd.digital-winds");
    TipoMimeType.set(".dts", "audio/vnd.dts");
    TipoMimeType.set(".dtshd", "audio/vnd.dts.hd");
    TipoMimeType.set(".lvp", "audio/vnd.lucent.voice");
    TipoMimeType.set(".pya", "audio/vnd.ms-playready.media.pya");
    TipoMimeType.set(".ecelp4800", "audio/vnd.nuera.ecelp4800");
    TipoMimeType.set(".ecelp7470", "audio/vnd.nuera.ecelp7470");
    TipoMimeType.set(".ecelp9600", "audio/vnd.nuera.ecelp9600");
    TipoMimeType.set(".aac", "audio/x-aac");
    TipoMimeType.set(".aif,.aiff,.aifc", "audio/x-aiff");
    TipoMimeType.set(".m3u", "audio/x-mpegurl");
    TipoMimeType.set(".wax", "audio/x-ms-wax");
    TipoMimeType.set(".wma", "audio/x-ms-wma");
    TipoMimeType.set(".ram,.ra", "audio/x-pn-realaudio");
    TipoMimeType.set(".rmp", "audio/x-pn-realaudio-plugin");
    TipoMimeType.set(".wav", "audio/x-wav");
    TipoMimeType.set(".cdx", "chemical/x-cdx");
    TipoMimeType.set(".cif", "chemical/x-cif");
    TipoMimeType.set(".cmdf", "chemical/x-cmdf");
    TipoMimeType.set(".cml", "chemical/x-cml");
    TipoMimeType.set(".csml", "chemical/x-csml");
    TipoMimeType.set(".xyz", "chemical/x-xyz");
    TipoMimeType.set(".bmp", "image/bmp");
    TipoMimeType.set(".cgm", "image/cgm");
    TipoMimeType.set(".g3", "image/g3fax");
    TipoMimeType.set(".gif", "image/gif");
    TipoMimeType.set(".ief", "image/ief");
    TipoMimeType.set(".jp2", "image/jp2");
    TipoMimeType.set(".jpeg,.jpg,.jpe", "image/jpeg");
    TipoMimeType.set(".pict,.pic,.pct", "image/pict");
    TipoMimeType.set(".png", "image/png");
    TipoMimeType.set(".btif", "image/prs.btif");
    TipoMimeType.set(".svg,.svgz", "image/svg+xml");
    TipoMimeType.set(".tiff,.tif", "image/tiff");
    TipoMimeType.set(".psd", "image/vnd.adobe.photoshop");
    TipoMimeType.set(".djvu,.djv", "image/vnd.djvu");
    TipoMimeType.set(".dwg", "image/vnd.dwg");
    TipoMimeType.set(".dxf", "image/vnd.dxf");
    TipoMimeType.set(".fbs", "image/vnd.fastbidsheet");
    TipoMimeType.set(".fpx", "image/vnd.fpx");
    TipoMimeType.set(".fst", "image/vnd.fst");
    TipoMimeType.set(".mmr", "image/vnd.fujixerox.edmics-mmr");
    TipoMimeType.set(".rlc", "image/vnd.fujixerox.edmics-rlc");
    TipoMimeType.set(".mdi", "image/vnd.ms-modi");
    TipoMimeType.set(".npx", "image/vnd.net-fpx");
    TipoMimeType.set(".wbmp", "image/vnd.wap.wbmp");
    TipoMimeType.set(".xif", "image/vnd.xiff");
    TipoMimeType.set(".ras", "image/x-cmu-raster");
    TipoMimeType.set(".cmx", "image/x-cmx");
    TipoMimeType.set(".fh,.fhc,.fh4,.fh5,.fh7", "image/x-freehand");
    TipoMimeType.set(".ico", "image/x-icon");
    TipoMimeType.set(".pntg,.pnt,.mac", "image/x-macpaint");
    TipoMimeType.set(".pcx", "image/x-pcx");
    //TipoMimeType.set(".pic,.pct", "image/x-pict");
    TipoMimeType.set(".pnm", "image/x-portable-anymap");
    TipoMimeType.set(".pbm", "image/x-portable-bitmap");
    TipoMimeType.set(".pgm", "image/x-portable-graymap");
    TipoMimeType.set(".ppm", "image/x-portable-pixmap");
    TipoMimeType.set(".qtif,.qti", "image/x-quicktime");
    TipoMimeType.set(".rgb", "image/x-rgb");
    TipoMimeType.set(".xbm", "image/x-xbitmap");
    TipoMimeType.set(".xpm", "image/x-xpixmap");
    TipoMimeType.set(".xwd", "image/x-xwindowdump");
    TipoMimeType.set(".eml,.mime", "message/rfc822");
    TipoMimeType.set(".igs,.iges", "model/iges");
    TipoMimeType.set(".msh,.mesh,.silo", "model/mesh");
    TipoMimeType.set(".dwf", "model/vnd.dwf");
    TipoMimeType.set(".gdl", "model/vnd.gdl");
    TipoMimeType.set(".gtw", "model/vnd.gtw");
    TipoMimeType.set(".mts", "model/vnd.mts");
    TipoMimeType.set(".vtu", "model/vnd.vtu");
    TipoMimeType.set(".wrl,.vrml", "model/vrml");
    TipoMimeType.set(".ics,.ifb", "text/calendar");
    TipoMimeType.set(".css", "text/css");
    TipoMimeType.set(".csv", "text/csv");
    TipoMimeType.set(".html,.htm", "text/html");
    TipoMimeType.set(".txt,.text,.conf,.def,.list,.log,.in", "text/plain");
    TipoMimeType.set(".dsc", "text/prs.lines.tag");
    TipoMimeType.set(".rtx", "text/richtext");
    TipoMimeType.set(".sgml,.sgm", "text/sgml");
    TipoMimeType.set(".tsv", "text/tab-separated-values");
    TipoMimeType.set(".t,.tr,.roff,.man,.me,.ms", "text/troff");
    TipoMimeType.set(".uri,.uris,.urls", "text/uri-list");
    TipoMimeType.set(".curl", "text/vnd.curl");
    TipoMimeType.set(".dcurl", "text/vnd.curl.dcurl");
    TipoMimeType.set(".scurl", "text/vnd.curl.scurl");
    TipoMimeType.set(".mcurl", "text/vnd.curl.mcurl");
    TipoMimeType.set(".fly", "text/vnd.fly");
    TipoMimeType.set(".flx", "text/vnd.fmi.flexstor");
    TipoMimeType.set(".gv", "text/vnd.graphviz");
    TipoMimeType.set(".3dml", "text/vnd.in3d.3dml");
    TipoMimeType.set(".spot", "text/vnd.in3d.spot");
    TipoMimeType.set(".jad", "text/vnd.sun.j2me.app-descriptor");
    TipoMimeType.set(".wml", "text/vnd.wap.wml");
    TipoMimeType.set(".wmls", "text/vnd.wap.wmlscript");
    TipoMimeType.set(".s,.asm", "text/x-asm");
    TipoMimeType.set(".c,.cc,.cxx,.cpp,.h,.hh,.dic", "text/x-c");
    TipoMimeType.set(".f,.for,.f77,.f90", "text/x-fortran");
    TipoMimeType.set(".p,.pas", "text/x-pascal");
    TipoMimeType.set(".java", "text/x-java-source");
    TipoMimeType.set(".etx", "text/x-setext");
    TipoMimeType.set(".uu", "text/x-uuencode");
    TipoMimeType.set(".vcs", "text/x-vcalendar");
    TipoMimeType.set(".vcf", "text/x-vcard");
    TipoMimeType.set(".3gp", "video/3gpp");
    TipoMimeType.set(".3g2", "video/3gpp2");
    TipoMimeType.set(".h261", "video/h261");
    TipoMimeType.set(".h263", "video/h263");
    TipoMimeType.set(".h264", "video/h264");
    TipoMimeType.set(".jpgv", "video/jpeg");
    TipoMimeType.set(".jpm,.jpgm", "video/jpm");
    TipoMimeType.set(".mj2,.mjp2", "video/mj2");
    TipoMimeType.set(".mp4,.mp4v,.mpg4,.m4v", "video/mp4");
    TipoMimeType.set(".webm", "video/webm");
    TipoMimeType.set(".mpeg,.mpg,.mpe,.m1v,.m2v", "video/mpeg");
    TipoMimeType.set(".ogv", "video/ogg");
    TipoMimeType.set(".qt,.mov", "video/quicktime");
    TipoMimeType.set(".fvt", "video/vnd.fvt");
    TipoMimeType.set(".mxu,.m4u", "video/vnd.mpegurl");
    TipoMimeType.set(".pyv", "video/vnd.ms-playready.media.pyv");
    TipoMimeType.set(".viv", "video/vnd.vivo");
    TipoMimeType.set(".dv,.dif", "video/x-dv");
    TipoMimeType.set(".f4v", "video/x-f4v");
    TipoMimeType.set(".fli", "video/x-fli");
    TipoMimeType.set(".flv", "video/x-flv");
    //TipoMimeType.set(".m4v", "video/x-m4v");
    TipoMimeType.set(".asf,.asx", "video/x-ms-asf");
    TipoMimeType.set(".wm", "video/x-ms-wm");
    TipoMimeType.set(".wmv", "video/x-ms-wmv");
    TipoMimeType.set(".wmx", "video/x-ms-wmx");
    TipoMimeType.set(".wvx", "video/x-ms-wvx");
    TipoMimeType.set(".avi", "video/x-msvideo");
    TipoMimeType.set(".movie", "video/x-sgi-movie");
    TipoMimeType.set(".ice", "x-conference/x-cooltalk");
    TipoMimeType.set(".indd", "application/x-indesign");
    TipoMimeType.set(".dat", "application/octet-stream");

    // Compressed files
    // Based on notes at http://en.wikipedia.org/wiki/List_of_archive_formats
    TipoMimeType.set(".gz", "application/x-gzip");
    TipoMimeType.set(".tgz", "application/x-tar");
    TipoMimeType.set(".tar", "application/x-tar");

    // Not really sure about these...
    TipoMimeType.set(".epub", "application/epub+zip");
    TipoMimeType.set(".mobi", "application/x-mobipocket-ebook");

    // Here's some common special cases without filename extensions
    TipoMimeType.set("README,LICENSE,COPYING,TODO,ABOUT,AUTHORS,CONTRIBUTORS",
		"text/plain");
    TipoMimeType.set("manifest,.manifest,.mf,.appcache", "text/cache-manifest");
    

    if (self.TipoMimeType === undefined) {
        self.TipoMimeType = TipoMimeType;
    }

    return self;
}(this));