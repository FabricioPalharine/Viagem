(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('RefeicaoEditCtrl', ['Error', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Cidade', 'Viagem', 'Atracao', 'Foto', '$uibModal', '$timeout', 'Gasto', 'SignalR', 'Refeicao', RefeicaoEditCtrl]);

	function RefeicaoEditCtrl(Error, $state, $translate, $scope, Auth, $rootScope, $stateParams, Cidade, Viagem, Atracao, Foto, $uibModal, $timeout, Gasto, SignalR, Refeicao) {
	    var vm = this;
	    vm.itemRefeicao = {};
	    vm.loading = false;
	    vm.messages = [];
	    vm.loggedUser = Auth.currentUser;
	    vm.CamposInvalidos = {};
	    vm.ListaParticipante = [];
	    vm.ItemAvaliacao = {};
	    vm.slickLoaded = false;
	    vm.itemOriginal = {};

	    vm.ajustaInicio = function (item) {
	        vm.itemOriginal = vm.itemRefeicao = item;
	        if (!item.Identificador) {
	            angular.forEach($scope.$parent.itemRefeicao.ListaParticipantes, function (c) {
	                var item2 = jQuery.extend({}, c);
	                item2.Selecionado = true;
	                vm.ListaParticipante.push(item2);
	            });
	        }
	    };

	    vm.load = function (itemBase) {
	        if (itemBase.Identificador && !vm.itemRefeicao.Pedidos && !vm.loading) {
	            vm.loading = true;
	            Refeicao.get({ id: itemBase.Identificador }, function (data) {
	                vm.itemRefeicao = data;
	                angular.forEach($scope.$parent.itemRefeicao.ListaParticipantes, function (c) {
	                    var item = jQuery.extend({}, c);
	                    item.Selecionado = vm.itemRefeicao.Pedidos && $.grep(vm.itemRefeicao.Pedidos, function (e) {
	                        if (e.IdentificadorUsuario == item.Identificador) {
	                            if (e.IdentificadorUsuario == Auth.currentUser.Codigo) {
	                                vm.ItemAvaliacao.Nota = e.Nota;
	                                vm.ItemAvaliacao.Comentario = e.Comentario;
	                            }
	                            item.Pedido = e.Pedido;
	                        }
	                        return e.IdentificadorUsuario == item.Identificador;
	                    }).length > 0;
	                    vm.ListaParticipante.push(item);
	                });
	                vm.RecarregarFotos();
	                vm.loading = false;
	            });

	        }
	    };

	    vm.save = function () {
	        vm.messages = [];
	        vm.submitted = true;
	        vm.loading = true;
	        vm.CamposInvalidos = {};
	        {

	            if (vm.itemRefeicao.ItemAtracao && vm.itemRefeicao.ItemAtracao.Identificador)
	                vm.itemRefeicao.IdentificadorAtracao = vm.itemRefeicao.ItemAtracao.Identificador
	            else {
	                vm.itemRefeicao.IdentificadorAtracao = null;
	                vm.itemRefeicao.ItemAtracao = null;
	            }

	            if (vm.itemRefeicao.Data) {
	                if (typeof vm.itemRefeicao.Data == "string") {

	                    vm.itemRefeicao.Data = moment(vm.itemRefeicao.Data).format("YYYY-MM-DDT");
	                }
	                else
	                    vm.itemRefeicao.Data = moment(vm.itemRefeicao.Data).format("YYYY-MM-DDT");
	                vm.itemRefeicao.Data += (vm.itemRefeicao.strHora) ? vm.itemRefeicao.strHora : "00:00:00";

	            }

	            
	            angular.forEach(vm.ListaParticipante, function (item) {
	                var itens =
                         $.grep(vm.itemRefeicao.Pedidos, function (e) { return e.IdentificadorUsuario == item.Identificador && !e.DataExclusao });
	                if (item.Selecionado && itens.length == 0) {
	                    var NovoItem = { IdentificadorUsuario: item.Identificador, DataAtualizacao: moment(new Date()).format("YYYY-MM-DDTHH:mm:ss"), Pedido: item.Pedido }
	                    vm.itemRefeicao.Pedidos.push(NovoItem);
	                }
	                else if (!item.Selecionado && itens.length > 0) {

	                    item.DataExclusao = moment(new Date()).format("YYYY-MM-DDTHH:mm:ss");
	                }

	            });

	            if (vm.itemRefeicao.Nome == "")
	                vm.itemRefeicao.Nome = null;

	            var MinhaAvaliacao =
                         $.grep(vm.itemRefeicao.Pedidos, function (e) { return e.IdentificadorUsuario == Auth.currentUser.Codigo && !e.DataExclusao });
	            if (MinhaAvaliacao.length > 0) {
	                MinhaAvaliacao[0].DataAtualizacao = moment(new Date()).format("YYYY-MM-DDTHH:mm:ss");
	                MinhaAvaliacao[0].Comentario = vm.ItemAvaliacao.Comentario;
	                MinhaAvaliacao[0].Nota = vm.ItemAvaliacao.Nota;
	            }
	            angular.forEach(vm.itemRefeicao.Fotos, function (item) {
	                if (item.ItemFoto != null)
	                    item.ItemFoto.Refeicoes = null;
	            });
	            angular.forEach(vm.itemRefeicao.Gastos, function (item) {
	                if (item.ItemGasto != null)
	                    item.ItemGasto.Refeicoes = null;
	            });
	            Refeicao.save(vm.itemRefeicao, function (data) {
	                vm.loading = false;
	                if (data.Sucesso) {
	                    Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
	                    $scope.$parent.itemRefeicao.AjustarRefeicaoSalva(vm.itemOriginal, data.ItemRegistro);
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
	        vm.itemRefeicao.CodigoPlace = $item.place_id;
	       
	        vm.itemRefeicao.Latitude = $item.geometry.location.lat();
	        vm.itemRefeicao.Longitude = $item.geometry.location.lng();

	        var request = {
	            placeId: $item.place_id
	        };
	        if (!vm.itemRefeicao.Tipo) {
	            var service = new google.maps.places.PlacesService(document.createElement('div'));
	            service.getDetails(request, function (place, status) {
	                if (status == google.maps.places.PlacesServiceStatus.OK) {

	                }
	            });
	        }
	    };

	    vm.PesquisarDadosGoogleApi = function (valor, callback, error) {
	        if (vm.itemRefeicao.Latitude && vm.itemRefeicao.Longitude) {
	            var posicao = new google.maps.LatLng(vm.itemRefeicao.Latitude, vm.itemRefeicao.Longitude);
	            var request = {
	                location: posicao,
	                radius: '2500',
	                name: valor,
	                types: ['bakery','bar','cafe','food','grocery_or_supermarket','liquor_store','meal_delivery','meal_takeaway','restaurant']
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
	        return $scope.$parent.itemRefeicao.ListaAtracao;
	    };


	
	    vm.VerificarParticipoRefeicao = function () {
	        return $.grep(vm.ListaParticipante, function (e) {

	            return e.Identificador == Auth.currentUser.Codigo && e.Selecionado;
	        }).length > 0;
	    };

	    vm.RemoverCusto = function (itemCusto) {
	        $scope.$parent.itemRefeicao.modalPopupTrigger(itemCusto, $translate.instant('MensagemExclusao'), $translate.instant('Sim'), $translate.instant('Nao'), function () {
	            itemCusto.DataExclusao = moment(new Date()).format("YYYY-MM-DDTHH:mm:ss");
	            Gasto.SalvarCustoRefeicao(itemCusto);
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
	                    ImageMime: item.type, DataArquivo: item.lastModifiedDate, CodigoGoogle: uploadVideo.videoId, Thumbnail: uploadVideo.thumbnail, IdentificadorRefeicao: vm.itemRefeicao.Identificador,
	                    LinkGoogle: link
	                };
	                if (status == null) {
	                    itemArquivo.Situacao = $translate.instant('Foto_Enviando');
	                    Foto.SubirVideo(itemFoto, function (data) {
	                        itemArquivo.Situacao = $translate.instant('Foto_Sucesso');
	                        itemArquivo.CodigoSituacao = 1;
	                        vm.itemRefeicao.Fotos.push({ IdentificadorFoto: data.ItemRegistro.Identificador, ItemFoto: data.ItemRegistro, DataAtualizacao: moment(new Date()).format("YYYY-MM-DDTHH:mm:ss"), Identificador: $.grep(data.ItemRegistro.Refeicoes, function (e) { return e.IdentificadorRefeicao == vm.itemRefeicao.Identificador && !e.DataExclusao })[0].Identificador });
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
	        var itemPush = itemGasto.Refeicoes[0];
	        itemPush.ItemGasto = itemGasto;
	        vm.itemRefeicao.Gastos.push(itemPush);
	        SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'G', itemGasto.Identificador, true);
	    };

	    vm.AbrirGasto = function () {
	        $scope.$parent.itemRefeicao.modalPopupTrigger(vm.itemRefeicao, $translate.instant('MensagemGastoAdicionar'), $translate.instant('Novo'), $translate.instant('Existente'), function () {
	            var Referencias = { IdentificadorRefeicao: vm.itemRefeicao.Identificador };
	            $uibModal.open({
	                templateUrl: 'EditaCusto.html',
	                controller: 'GastoEditCtrl',
	                controllerAs: 'itemGastoEdit',
	                resolve: {
	                    Referencias: function () { return Referencias; },
	                    EscopoAtualizacao: vm
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
                    var itemFoto = { Base64: imageURL, ImageMime: item.type, DataArquivo: item.lastModifiedDate, IdentificadorRefeicao: vm.itemRefeicao.Identificador };
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
                        vm.itemRefeicao.Fotos.push({ IdentificadorFoto: data.ItemRegistro.Identificador, ItemFoto: data.ItemRegistro, DataAtualizacao: moment(new Date()).format("YYYY-MM-DDTHH:mm:ss"), Identificador: $.grep(data.ItemRegistro.Refeicoes, function (e) { return e.IdentificadorRefeicao == vm.itemRefeicao.Identificador && !e.DataExclusao })[0].Identificador });
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
	        $scope.$parent.itemRefeicao.modalPopupTrigger(vm.itemRefeicao, $translate.instant('MensagemExclusao'), $translate.instant('Sim'), $translate.instant('Nao'), function () {
	            $scope.$parent.itemRefeicao.Excluir(vm.itemRefeicao)
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
	        $timeout(function () { vm.slickLoaded = vm.itemRefeicao.Fotos && vm.itemRefeicao.Fotos.length > 0; }, 500);
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
	            $scope.$parent.itemRefeicao.modalPopupTrigger(vmEdit.itemFoto, $translate.instant('MensagemExclusao'), $translate.instant('Sim'), $translate.instant('Nao'), function () {
	                vm.itemRefeicao.ChegadaExclusao = moment(new Date()).format("YYYY-MM-DDTHH:mm:ss");
	                Foto.saveFotoRefeicao(vmEdit.itemFoto);
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
	                    item: function () { return vmEdit.itemFoto.ItemFoto; },
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
	                item: function () { return vm.itemRefeicao; },
	            }
	        });
	    };


	    vm.SelecionaCustoCtrl = function ($uibModalInstance) {
	        var vmSelecao = this;
	        vmSelecao.ListaParticipante = vm.ListaParticipante;
	        vmSelecao.filtro = {};
	        vmSelecao.itemUsuario = null;
	        vmSelecao.ListaGastos = [];

	        vmSelecao.Idioma = function () {
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

	            if (vmSelecao.filtro.DataInicioAte) {
	                if (typeof vmSelecao.filtro.DataInicioAte == "string") {

	                }
	                else
	                    vmSelecao.filtro.DataInicioAte = $.datepicker.formatDate("yy-mm-ddT00:00:00", vmSelecao.filtro.DataInicioAte);
	            }

	            Gasto.list({ json: JSON.stringify(vmSelecao.filtro) }, function (data) {
	                vmSelecao.ListaGastos = data.Lista;
	            });
	        };

	        vmSelecao.SelecionarCusto = function (itemCusto) {
	            var itemGravar = { IdentificadorRefeicao: vm.itemRefeicao.Identificador, IdentificadorGasto: itemCusto.Identificador, DataAtualizacao: moment(new Date()).format("YYYY-MM-DDTHH:mm:ss") };
	            Gasto.SalvarCustoRefeicao(itemGravar, function (data) {
	                if (data.Sucesso) {
	                    var itemPush = { Identificador: data.IdentificadorRegistro, ItemGasto: itemCusto };
	                    vm.itemRefeicao.Gastos.push(itemPush);
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
