(function () {
    'use strict';
    angular
		.module('Sistema')
		.controller('AtracaoEditCtrl',[ 'Error',  '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Cidade','Viagem','Atracao','Foto','$uibModal','$timeout','Gasto','SignalR', AtracaoEditCtrl]);

    function AtracaoEditCtrl(Error, $state, $translate, $scope, Auth, $rootScope, $stateParams, Cidade, Viagem, Atracao, Foto, $uibModal, $timeout,Gasto,SignalR) {
        var vm = this;
        vm.itemAtracao = {};
        vm.loading = false;
        vm.messages = [];
        vm.loggedUser = Auth.currentUser;
        vm.CamposInvalidos = {};
        vm.ListaParticipante = [];
        vm.VisitaIniciada = false;
        vm.VisitaConcluida = false;
        vm.ItemAvaliacao = {};
        vm.slickLoaded = false;
        vm.itemOriginal = {};

        vm.position = null;


        vm.AjustarPosicao = function (position) {
            vm.position = {};
            vm.position.lat = position.coords.latitude;
            vm.position.lng = position.coords.longitude;
        };

        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(vm.AjustarPosicao);
        }

        vm.ajustaInicio = function (item) {
            vm.itemOriginal = vm.itemAtracao = item;
            if (!item.Identificador)
            {
                angular.forEach($scope.$parent.itemAtracao.ListaParticipantes, function (c) {
                    var item2 = jQuery.extend({}, c);
                    item2.Selecionado = false;
                    vm.ListaParticipante.push(item2);
                });
            }
        };

        vm.load = function (item) {
            if (item.Identificador && !vm.itemAtracao.Avaliacoes && !vm.loading ) {
                vm.loading = true;
                Atracao.get({ id: item.Identificador }, function (data) {
                    vm.itemAtracao = data;
                    angular.forEach($scope.$parent.itemAtracao.ListaParticipantes, function (c) {
                        var item = jQuery.extend({}, c);
                        item.Selecionado = vm.itemAtracao.Avaliacoes && $.grep(vm.itemAtracao.Avaliacoes, function (e) {
                            if (e.IdentificadorUsuario == item.Identificador) {
                                if (e.IdentificadorUsuario == Auth.currentUser.Codigo) {
                                    vm.ItemAvaliacao.Nota = e.Nota;
                                    vm.ItemAvaliacao.Comentario = e.Comentario;
                                }
                            }
                            return e.IdentificadorUsuario == item.Identificador;
                        }).length > 0;
                        vm.ListaParticipante.push(item);
                    });
                    vm.VisitaIniciada = item.Chegada != null;
                    vm.VisitaConcluida = item.Partida != null;
                    vm.RecarregarFotos();
                    vm.loading = false;
                });

            }
        };

        vm.save = function () {
            vm.messages = [];
            vm.submitted = true;
            vm.CamposInvalidos = {};
            vm.loading = true;
            {
              
                if (vm.itemAtracao.ItemAtracaoPai && vm.itemAtracao.ItemAtracaoPai.Identificador)
                    vm.itemAtracao.IdentificadorAtracaoPai = vm.itemAtracao.ItemAtracaoPai.Identificador
                else {
                    vm.itemAtracao.IdentificadorAtracaoPai = null;
                    vm.itemAtracao.ItemAtracaoPai = null;
                }

                if (  vm.itemAtracao.Chegada) {
                    if (typeof   vm.itemAtracao.Chegada == "string") {

                        vm.itemAtracao.Chegada = moment(vm.itemAtracao.Chegada).format("YYYY-MM-DDT");
                    }
                    else
                        vm.itemAtracao.Chegada = moment(vm.itemAtracao.Chegada).format("YYYY-MM-DDT");
                    vm.itemAtracao.Chegada += (vm.itemAtracao.strHoraChegada) ? vm.itemAtracao.strHoraChegada : "00:00:00";

                }

                if ( vm.itemAtracao.Partida) {
                    if (typeof  vm.itemAtracao.Partida == "string") {
                        vm.itemAtracao.Partida = moment(vm.itemAtracao.Partida).format("YYYY-MM-DDT");
                    }
                    else
                        vm.itemAtracao.Partida = moment(vm.itemAtracao.Partida).format("YYYY-MM-DDT");
                    vm.itemAtracao.Partida += (vm.itemAtracao.strHoraPartida) ? vm.itemAtracao.strHoraPartida : "00:00:00";

                }
                angular.forEach(vm.ListaParticipante, function (item) {
                    var itens =
                         $.grep(vm.itemAtracao.Avaliacoes, function (e) { return e.IdentificadorUsuario == item.Identificador && !e.DataExclusao });
                    if (item.Selecionado && itens.length == 0) {
                        var NovoItem = { IdentificadorUsuario: item.Identificador, DataAtualizacao: moment(new Date()).format("YYYY-MM-DDTHH:mm:ss") }
                        vm.itemAtracao.Avaliacoes.push(NovoItem);
                    }
                    else if (!item.Selecionado && itens.length > 0) {
                        //var posicao = vmEdit.itemFoto.Hoteis.indexOf(itens[0]);
                        //vmEdit.itemFoto.Hoteis.splice(posicao, 1);
                        item.DataExclusao = moment(new Date()).format("YYYY-MM-DDTHH:mm:ss");
                    }

                });

                if (vm.itemAtracao.Nome == "")
                    vm.itemAtracao.Nome = null;
                angular.forEach(vm.itemAtracao.Fotos, function (item) {
                    if (item.ItemFoto != null)
                        item.ItemFoto.Atracoes = null;
                });
                angular.forEach(vm.itemAtracao.Gastos, function (item) {
                    if (item.ItemGasto != null)
                        item.ItemGasto.Atracoes = null;
                });
                var MinhaAvaliacao =
                         $.grep(vm.itemAtracao.Avaliacoes, function (e) { return e.IdentificadorUsuario == Auth.currentUser.Codigo && !e.DataExclusao });
                if (MinhaAvaliacao.length > 0)
                {
                    MinhaAvaliacao[0].DataAtualizacao = moment(new Date()).format("YYYY-MM-DDTHH:mm:ss");
                    MinhaAvaliacao[0].Comentario = vm.ItemAvaliacao.Comentario;
                    MinhaAvaliacao[0].Nota = vm.ItemAvaliacao.Nota;
                }
                Atracao.save(vm.itemAtracao, function (data) {
                    vm.loading = false;
                    if (data.Sucesso) {
                        Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
                        $scope.$parent.itemAtracao.AjustarAtracaoSalva(vm.itemOriginal, data.ItemRegistro);
                    } else {
                        vm.messages = data.Mensagens;
                        vm.verificaCampoInvalido();
                    }
                }, function (err) {
                    vm.loading = false;
                    Error.showError('error', 'Ops!', $translate.instant("ErroSalvar"), true);
                });
            }
            vm.submitted = false;
        };

        vm.Idioma = function () {
            if (Auth && Auth.currentUser && Auth.currentUser.Cultura)
                return Auth.currentUser.Cultura.toLowerCase().substr(0, 2);
            else
                return "pt";
        };

        vm.verificaCampoInvalido = function () {
            vm.CamposInvalidos = {

            };
            //  var _retorno = false;
            $(vm.messages).each(function (i, item) {
                vm.CamposInvalidos[item.Campo] = true;
            });
        };


        vm.CarregarDadosGoogleApiPromise = function (valor) {
            var d = $.Deferred();
            vm.PesquisarDadosGoogleApi(valor, function (res) { d.resolve(res); }, function (err) { d.reject(err); });
            return d.promise();
        }

        vm.CarregarDadosGoogleApi = function (valor) {
            return vm.CarregarDadosGoogleApiPromise(valor)
            .then(function (response) {
                return response;
            });
        };

        vm.SelecionarPlacesGoogle = function ($item, $model, $label) {
            vm.itemAtracao.CodigoPlace = $item.place_id;
            if (!vm.itemAtracao.Tipo)
                vm.itemAtracao.Tipo = $item.types.join(',');
            vm.itemAtracao.Latitude = $item.geometry.location.lat();
            vm.itemAtracao.Longitude = $item.geometry.location.lng();
        };

        vm.PesquisarDadosGoogleApi = function (valor, callback, error) {
            if (vm.itemAtracao.Latitude && vm.itemAtracao.Longitude || vm.position.lat) {
                var posicao;
                if (vm.itemAtracao.Latitude && vm.itemAtracao.Longitude)
                    posicao = new google.maps.LatLng(vm.itemAtracao.Latitude, vm.itemAtracao.Longitude);
                else
                    posicao = new google.maps.LatLng(vm.position.lat, vm.position.lng);

                var request = {
                    location: posicao,
                    radius: '2500',
                    name: valor
                };
                var service = new google.maps.places.PlacesService(document.createElement('div'));
                service.nearbySearch(request, function (results, status) {
                    if (status == google.maps.places.PlacesServiceStatus.OK) {

                        callback(results);
                    }
                });
            }




        };

        vm.ListaAtracao = function () {
            return $.grep($scope.$parent.itemAtracao.ListaAtracao, function (e) {

                return e.Identificador !=vm.itemAtracao.Identificador;
            })
        };


        vm.AjustarHoraChegada = function () {
            if (vm.VisitaIniciada) {
                vm.itemAtracao.Chegada = moment(new Date()).format("YYYY-MM-DDTHH:mm:ss");
                vm.itemAtracao.strHoraChegada = moment(new Date()).format("HH:mm:ss");
                angular.forEach(vm.ListaParticipante, function (item) {
                    item.Selecionado = true;
                });
            }
            else {
                vm.itemAtracao.Chegada = null;
                vm.itemAtracao.strHoraChegada = moment(new Date()).format("HH:mm:ss");

                vm.itemAtracao.Partida = vm.itemAtracao.strHoraPartida = null;
                vm.VisitaConcluida = false;
                angular.forEach(vm.ListaParticipante, function (item) {
                    item.Selecionado = false;
                });
            }
        };

        vm.AjustarHoraPartida = function () {
            if (vm.VisitaConcluida) {
                vm.itemAtracao.Partida = moment(new Date()).format("YYYY-MM-DDTHH:mm:ss");
                vm.itemAtracao.strHoraPartida = moment(new Date()).format("HH:mm:ss");
              
            }
            else {
                vm.itemAtracao.Partida = vm.itemAtracao.strHoraPartida = null;
               
            }
        };

        vm.VerificarParticipoAtracao = function () {
            return $.grep(vm.ListaParticipante, function (e) {

                return e.Identificador == Auth.currentUser.Codigo && e.Selecionado;
            }).length > 0;
        };

        vm.RemoverCusto = function (itemCusto) {
            $scope.$parent.itemAtracao.modalPopupTrigger(itemCusto, $translate.instant('MensagemExclusao'), $translate.instant('Sim'), $translate.instant('Nao'), function () {
                itemCusto.DataExclusao = moment(new Date()).format("YYYY-MM-DDTHH:mm:ss");
                Gasto.SalvarCustoAtracao(itemCusto);
            });

        };

        vm.CarregarArquivos = function (files) {
            for (var i = 0; i < files.length; i++)
                vm.CarregarArquivo(files[i]);

        };

        vm.CarregarArquivo = function (item) {
            var itemArquivo = { FileName: item.name, Porcentual: 0, Situacao: $translate.instant('Foto_Carregando'), CodigoSituacao: 0 }
            var type = item.type;
            if (type.indexOf("image/") < 0
                && type.indexOf("video/") < 0
                && type.indexOf("application/x-zip-compressed") < 0) {
                itemArquivo.CodigoSituacao = 2;
                itemArquivo.Situacao = $translate.instant('Foto_ArquivoInvalido').format(item.name);


            }
            else {
                if (type.indexOf("image/") >= 0) {

                    vm.ProcessarImagem(item, itemArquivo);
                }
                else if (type.indexOf("video/") >= 0) {
                    vm.ProcessarVideo(item, itemArquivo);
                }
                else
                    vm.ProcessarZip(item);
            }
        };

        vm.ProcessarZip = function (file) {
            var reader = new FileReader();

            reader.onload = function (e) {
                JSZip.loadAsync(reader.result)
                .then(function (zip) {
                    zip.forEach(function (relativePath, itemzip) {
                        if (!itemzip.dir)
                            vm.processarItemZip(itemzip);

                    });

                });


            };

            reader.readAsArrayBuffer(file);
        };

        vm.processarItemZip = function (itemZip) {


            itemZip.async("base64", function (meta) {
            })
           .then(function success(content) {

               var tipo = TipoMimeType.lookup(itemZip.name);
               var blob = vm.b64toBlob(content, tipo);
               blob.lastModifiedDate = itemZip.date;
               blob.name = itemZip.name;
               vm.CarregarArquivo(blob);

           }, function error(e) {
               // handle the error
           });

        };

        vm.b64toBlob = function (b64Data, contentType, sliceSize) {
            contentType = contentType || '';
            sliceSize = sliceSize || 512;

            var byteCharacters = atob(b64Data);
            var byteArrays = [];

            for (var offset = 0; offset < byteCharacters.length; offset += sliceSize) {
                var slice = byteCharacters.slice(offset, offset + sliceSize);

                var byteNumbers = new Array(slice.length);
                for (var i = 0; i < slice.length; i++) {
                    byteNumbers[i] = slice.charCodeAt(i);
                }

                var byteArray = new Uint8Array(byteNumbers);

                byteArrays.push(byteArray);
            }

            var blob = new Blob(byteArrays, { type: contentType });
            return blob;
        }

        vm.ProcessarVideo = function (item, itemArquivo) {
            Foto.RetornarAlbum(function (result) {
                itemArquivo.Situacao = $translate.instant('Foto_SubindoVideo');
                var uploadVideo = new UploadVideo();
                uploadVideo.itemArquivo = itemArquivo;
                uploadVideo.MensagemProcessando = $translate.instant('Foto_ProcessandoVideo');
                uploadVideo.callbackUpdate = function () {
                    $scope.$apply();
                };
                uploadVideo.ready(result[1], item.name, item, function (link, status) {
                    var itemFoto = {
                        ImageMime: item.type, DataArquivo: item.lastModifiedDate, CodigoGoogle: uploadVideo.videoId, Thumbnail: uploadVideo.thumbnail, IdentificadorAtracao: vm.itemAtracao.Identificador,
                        LinkGoogle: link
                    };
                    if (status == null) {
                        itemArquivo.Situacao = $translate.instant('Foto_Enviando');
                        Foto.SubirVideo(itemFoto, function (data) {
                            itemArquivo.Situacao = $translate.instant('Foto_Sucesso');
                            itemArquivo.CodigoSituacao = 1;
                            vm.itemAtracao.Fotos.push({ IdentificadorFoto: data.ItemRegistro.Identificador, ItemFoto: data.ItemRegistro, DataAtualizacao: moment(new Date()).format("YYYY-MM-DDTHH:mm:ss"), Identificador: $.grep(data.ItemRegistro.Atracoes, function (e) { return e.IdentificadorAtracao == vm.itemAtracao.Identificador && !e.DataExclusao })[0].Identificador });
                            vm.RecarregarFotos();
                            SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'F', data.ItemRegistro.Identificador, true);


                        });
                    }
                    else {
                        itemArquivo.Situacao = $translate.instant('Foto_ErroVideo').format(status);
                        itemArquivo.CodigoSituacao = 2;
                    }
                });
            })
        };

        vm.AtualizarGasto = function (itemGasto, itemOriginal) {
            var itemPush = itemGasto.Atracoes[0];
            itemPush.ItemGasto = itemGasto;
            vm.itemAtracao.Gastos.push(itemPush);
            SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'G', itemGasto.Identificador, true);
        };


        vm.AbrirGasto = function () {
            $scope.$parent.itemAtracao.modalPopupTrigger(vm.itemAtracao, $translate.instant('MensagemGastoAdicionar'), $translate.instant('Novo'), $translate.instant('Existente'), function () {
                var Referencias= { IdentificadorAtracao: vm.itemAtracao.Identificador};
                $uibModal.open({
                    templateUrl: 'EditaCusto.html',
                    controller: 'GastoEditCtrl',
                    controllerAs: 'itemGastoEdit',
                    resolve: {
                        Referencias: function () { return Referencias; },
                        EscopoAtualizacao : vm
                    }
                });
            }, function () {
                $uibModal.open({
                    templateUrl: 'Sistema/GastoSelecao',
                    controller: ['$uibModalInstance', vm.SelecionaCustoCtrl],
                    controllerAs: 'vmSelecaoGasto',
                    resolve: {
                       
                    }
                });
            });
        };

        vm.ProcessarImagem = function (item, itemArquivo) {
            loadImage(
                item,
                function (img, dados) {
                    var imageURL = img.toDataURL();
                    var itemFoto = { Base64: imageURL, ImageMime: item.type, DataArquivo: item.lastModifiedDate, IdentificadorAtracao: vm.itemAtracao.Identificador };
                    if (dados && dados.exif) {
                        var tags = dados.exif.getAll()
                        if (tags.GPSLatitude) {
                            var arrPosicao = tags.GPSLatitude.split(',');
                            itemFoto.Latitude = vm.ConverterDegressDecimal(arrPosicao[0], arrPosicao[1], arrPosicao[2] == "NaN" ? 0 : arrPosicao[2], tags.GPSLatitudeRef)
                        }
                        if (tags.GPSLongitude) {
                            var arrPosicao = tags.GPSLongitude.split(',');
                            itemFoto.Longitude = vm.ConverterDegressDecimal(arrPosicao[0], arrPosicao[1], arrPosicao[2] == "NaN" ? 0 : arrPosicao[2], tags.GPSLongitudeRef)
                        }
                    }
                    itemArquivo.Situacao = $translate.instant('Foto_Enviando');

                    Foto.SubirImagem(itemFoto, function (data) {
                        itemArquivo.Situacao = $translate.instant('Foto_Sucesso');
                        itemArquivo.CodigoSituacao = 1;
                        vm.itemAtracao.Fotos.push({ IdentificadorFoto: data.ItemRegistro.Identificador, ItemFoto: data.ItemRegistro, DataAtualizacao: moment(new Date()).format("YYYY-MM-DDTHH:mm:ss"), Identificador: $.grep(data.ItemRegistro.Atracoes, function (e) { return e.IdentificadorAtracao == vm.itemAtracao.Identificador && !e.DataExclusao })[0].Identificador });
                        vm.RecarregarFotos();
                        SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'F', data.ItemRegistro.Identificador, true);

                    });


                },
                { maxWidth: 2048, maxHeight: 2048, canvas: true, orientation: true } // Options
            );
        };

        vm.ConverterDegressDecimal = function (degrees, minutes, seconds, direction) {
            var dd = parseInt(degrees) + parseFloat(minutes) / 60 + parseFloat(seconds) / (60 * 60);

            if (direction == "S" || direction == "W") {
                dd = dd * -1;
            } // Don't do anything for N or E
            return dd;
        }

        vm.VerificarExclusao = function () {
            $scope.$parent.itemAtracao.modalPopupTrigger(vm.itemAtracao, $translate.instant('MensagemExclusao'), $translate.instant('Sim'), $translate.instant('Nao'), function () {
                $scope.$parent.itemAtracao.Excluir(vm.itemOriginal)
            });
        };

        vm.slickConfig = {
            infinite: true,
            speed: 0,
            slidesToShow: 5,
            slidesToScroll: 5,
            responsive: [
              {
                  breakpoint: 1024,
                  settings: {
                      slidesToShow: 4,
                      slidesToScroll: 4,
                      infinite: true,
                      dots: true
                  }
              },
                {
                    breakpoint: 800,
                    settings: {
                        slidesToShow: 3,
                        slidesToScroll: 3,
                        infinite: true,
                        dots: true
                    }
                },
              {
                  breakpoint: 600,
                  settings: {
                      slidesToShow: 2,
                      slidesToScroll: 2
                  }
              },
              {
                  breakpoint: 480,
                  settings: {
                      slidesToShow: 1,
                      slidesToScroll: 1
                  }
              }
              // You can unslick at a given breakpoint now by adding:
              // settings: "unslick"
              // instead of a settings object
            ]
        };

        vm.RecarregarFotos = function () {
            vm.slickLoaded = false;
            $timeout(function () { vm.slickLoaded = vm.itemAtracao.Fotos && vm.itemAtracao.Fotos.length > 0; }, 500);
        };
        vm.AbrirJanelaEdicaoFoto = function (itemFoto) {

            $uibModal.open({
                templateUrl: 'editaImagem.html',
                controller: ['$uibModalInstance', 'item', vm.EditModalCtrl],
                controllerAs: 'vmEdit',
                resolve: {
                    item: function () { return itemFoto; },
                }
            });
        };


        vm.EditModalCtrl = function ($uibModalInstance, item) {
            var vmEdit = this;
            vmEdit.loading = false;
            vmEdit.itemFoto = item;
            vmEdit.itemFotoOriginal = jQuery.extend({}, item);
            vmEdit.messages = [];
            vmEdit.CamposInvalidos = {};

            // console.log(itens);
            vmEdit.close = function () {
                vmEdit.itemFoto = vmEdit.itemFotoOriginal;
                $uibModalInstance.close();
            }
           
            vmEdit.Excluir = function () {
                $scope.$parent.itemAtracao.modalPopupTrigger(vmEdit.itemFoto, $translate.instant('MensagemExclusao'), $translate.instant('Sim'), $translate.instant('Nao'), function () {
                      vm.itemAtracao.ChegadaExclusao = moment(new Date()).format("YYYY-MM-DDTHH:mm:ss");
                    Foto.saveFotoAtracao(vmEdit.itemFoto);
                    vm.RecarregarFotos();
                    $uibModalInstance.close();
                });
                
            };

            vmEdit.Idioma = function () {
                if (Auth && Auth.currentUser && Auth.currentUser.Cultura)
                    return Auth.currentUser.Cultura.toLowerCase().substr(0, 2);
                else
                    return "pt";
            };


            vmEdit.salvar = function () {
                if (vmEdit.itemFoto.ItemFoto.Data) {
                    if (typeof vmEdit.itemFoto.ItemFoto.Data == "string") {
                        var date = Date.parse(vmEdit.itemFoto.ItemFoto.Data);
                        if (!isNaN(date))
                            vmEdit.itemFoto.ItemFoto.Data = moment(new Date(date)).format("YYYY-MM-DDT");
                    }
                    else
                        vmEdit.itemFoto.ItemFoto.Data = moment(vmEdit.itemFoto.ItemFoto.Data).format("YYYY-MM-DDT");
                    vmEdit.itemFoto.ItemFoto.Data += (vmEdit.itemFoto.ItemFoto.strHora) ? vmEdit.itemFoto.ItemFoto.strHora : "00:00:00";

                }

                Foto.saveFoto(vmEdit.itemFoto.ItemFoto, function (data) {
                    vmEdit.loading = false;
                    
                        $uibModalInstance.close();
                   
                }, function (err) {
                    vmEdit.loading = false;
                    Error.showError('error', 'Ops!', $translate.instant("ErroSalvar"), true);
                });
            };

            vmEdit.verificaCampoInvalido = function () {
                vm.CamposInvalidos = {

                };

                $(vm.messages).each(function (i, item) {
                    vm.CamposInvalidos[item.Campo] = true;
                });
            };

            vmEdit.abrirMapa = function () {
                $uibModal.open({
                    templateUrl: 'modalMapa.html',
                    controller: ['$uibModalInstance', 'NgMap', '$timeout', '$scope', 'item', vm.MapModalCtrl],
                    controllerAs: 'vmMapa',
                    resolve: {
                        item: function () { return  vmEdit.itemFoto.ItemFoto; },
                    }
                });
            };

        }

        vm.SelecionarPosicao = function () {
            $uibModal.open({
                templateUrl: 'modalMapa.html',
                controller: ['$uibModalInstance', 'NgMap', '$timeout', '$scope', 'item', vm.MapModalCtrl],
                controllerAs: 'vmMapa',
                resolve: {
                    item: function () { return vm.itemAtracao; },
                }
            });
        };


        vm.SelecionaCustoCtrl = function ($uibModalInstance)
        {
            var vmSelecao = this;
            vmSelecao.ListaParticipante = vm.ListaParticipante;
            vmSelecao.filtro = {};
            vmSelecao.itemUsuario = null;
            vmSelecao.ListaGastos = [];

            vmSelecao.Idioma = function ()
            {
                if (Auth && Auth.currentUser && Auth.currentUser.Cultura)
                    return Auth.currentUser.Cultura.toLowerCase().substr(0, 2);
                else
                    return "pt";
            };
            vmSelecao.filtraDado = function () {
                if (vmSelecao.itemUsuario && vmSelecao.itemUsuario.Identificador)
                    vmSelecao.filtro.IdentificadorUsuario = vmSelecao.itemUsuario.Identificador;
                else
                    vmSelecao.filtro.IdentificadorUsuario = null;

                if (vmSelecao.filtro.DataInicioDe) {
                    if (typeof vmSelecao.filtro.DataInicioDe == "string") {

                    }
                    else
                        vmSelecao.filtro.DataInicioDe = $.datepicker.formatDate("yy-mm-ddT00:00:00", vmSelecao.filtro.DataInicioDe);
                }

                if ( vmSelecao.filtro.DataInicioAte) {
                    if (typeof  vmSelecao.filtro.DataInicioAte == "string") {

                    }
                    else
                        vmSelecao.filtro.DataInicioAte = $.datepicker.formatDate("yy-mm-ddT00:00:00", vmSelecao.filtro.DataInicioAte);
                }

                Gasto.list({ json: JSON.stringify( vmSelecao.filtro) }, function (data) {
                    vmSelecao.ListaGastos = data.Lista;
                });
            };

            vmSelecao.SelecionarCusto = function (itemCusto) {
                var itemGravar = { IdentificadorAtracao: vm.itemAtracao.Identificador, IdentificadorGasto: itemCusto.Identificador, DataAtualizacao: moment(new Date()).format("YYYY-MM-DDTHH:mm:ss") };
                Gasto.SalvarCustoAtracao(itemGravar, function (data) {
                    if (data.Sucesso)
                    {
                        var itemPush = { Identificador: data.IdentificadorRegistro, ItemGasto: itemCusto };
                        vm.itemAtracao.Gastos.push(itemPush);
                    }
                    $uibModalInstance.close();
                });
            };
        }

        vm.MapModalCtrl = function ($uibModalInstance, NgMap, $timeout, $scope, item) {
            var vmMapa = this;
            vmMapa.lat = -23.6040963;
            vmMapa.lng = -46.6178018;
            vmMapa.itemEndereco = "";
            vmMapa.itemFoto = item;
            vmMapa.itemMarcador = {};
            vmMapa.map = null;


            vmMapa.AjustarPosicao = function (position) {
                vmMapa.lat = position.coords.latitude;
                vmMapa.lng = position.coords.longitude;
            };

            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(vmMapa.AjustarPosicao);
            }


            NgMap.getMap().then(function (evtMap) {
                vmMapa.map = evtMap;
                $timeout(function () {
                    google.maps.event.trigger(vmMapa.map, "resize");

                    if (vmMapa.itemFoto.Latitude && vmMapa.itemFoto.Longitude) {
                        vmMapa.lat = vmMapa.itemFoto.Latitude;
                        vmMapa.lng = vmMapa.itemFoto.Longitude;
                        vmMapa.itemMarcador = { Latitude: vmMapa.itemFoto.Latitude, Longitude: vmMapa.itemFoto.Longitude };
                    }
                    $scope.$apply();
                }, 500);
            });
            vmMapa.close = function () {
                $uibModalInstance.close();
            };

            vmMapa.centralizarEndereco = function () {
                var geocoder = new google.maps.Geocoder();
                geocoder.geocode({ 'address': vmMapa.itemEndereco }, function (results, status) {
                    if (status == google.maps.GeocoderStatus.OK) {
                        vmMapa.lat = results[0].geometry.location.lat();
                        vmMapa.lng = results[0].geometry.location.lng();


                        vmMapa.map.setCenter(results[0].geometry.location);
                    }
                });
            };


            vmMapa.selecionarEndereco = function () {
                var place = this.getPlace();
                vmMapa.itemEndereco = place.formatted_address;

                vmMapa.lat = place.geometry.location.lat();
                vmMapa.lng = place.geometry.location.lng();


                vmMapa.map.setCenter(place.geometry.location);

            };

            vmMapa.salvar = function () {
                vmMapa.itemFoto.Latitude = vmMapa.itemMarcador.Latitude;
                vmMapa.itemFoto.Longitude = vmMapa.itemMarcador.Longitude;
                $uibModalInstance.close();

            };

            vmMapa.ajustaPosicao = function (event) {
                var ll = event.latLng;
                vmMapa.itemMarcador = { Latitude: ll.lat(), Longitude: ll.lng() };

            };

            vmMapa.limparPosicao = function () {
                vmMapa.itemMarcador = {};
            };
        };
    }
}());
