(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('FotoCtrl', ['$uibModal', 'Error', '$timeout', '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', '$window', 'i18nService', 'Cidade', 'Usuario', 'Viagem', 'Foto',
            'Atracao','Hotel','Refeicao','Loja','NgMap','SignalR', FotoCtrl]);

	function FotoCtrl($uibModal, Error, $timeout, $state, $translate, $scope, Auth, $rootScope, $stateParams, $window, i18nService, Cidade, Usuario, Viagem, Foto, Atracao, Hotel, Refeicao, Loja, NgMap,SignalR) {
		var vm = this;
		vm.filtro = {  Index: 0, Count: 0,Atracoes: [], Hoteis: [], Refeicoes:[] };
		vm.filtroAtualizacao = {  Index: 0, Count: 0 };
		vm.loading = false;
		vm.showModal = false;
		vm.modalAcao = function () {;
			vm.showModal = true;
		}
		vm.modalDelete = {};
		
		vm.ListaDados = [];

		vm.ListaArquivos = [];
		vm.ListaHotel = [];
		vm.ListaAtracao = [];
		vm.ListaRefeicao = [];
		vm.ListaItemCompra = [];
		vm.ListaCidades = [];
		vm.itemCidade = null;
		vm.pageSize = 30
		vm.currentItem = 0;
		vm.enableScroll = false;
		vm.continuaPesquisando = true;
		vm.FimPagina = false;
		vm.CarregarArquivos = function (files) {
		    for (var i = 0; i < files.length; i++)
		        vm.CarregarArquivo(files[i]);

		};

		vm.CarregarArquivo = function (item) {
		    var itemArquivo = { FileName: item.name, Porcentual: 0, Situacao: $translate.instant('Foto_Carregando'), CodigoSituacao: 0 }
		    var type = item.type;
		    if (type.indexOf("image/") < 0 
                && type.indexOf("video/") < 0
                && type.indexOf("application/x-zip-compressed") < 0)
		    {
		        itemArquivo.CodigoSituacao = 2;
		        itemArquivo.Situacao= $translate.instant('Foto_ArquivoInvalido').format(item.name);
		        vm.ListaArquivos.push(itemArquivo);

		       
		    }
		    else
		    {
		        if (type.indexOf("image/") >= 0) {

		            vm.ListaArquivos.push(itemArquivo);
		            vm.ProcessarImagem(item, itemArquivo);
		        }
		        else if (type.indexOf("video/") >= 0) {
		            vm.ListaArquivos.push(itemArquivo);
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
		    //itemZip.async("arraybuffer")
            //.then(function success(content) {
            //    //var blob = new Blob(content, { name: itemZip.name, lastModifiedDate: itemZip.date });

            //}, function error(e) {
            //    // handle the error
            //});

		    itemZip.async("base64", function (meta) {
		        //console.log("Generating the content, we are at " + meta.percent.toFixed(2) + " %");
		    })
           .then(function success(content ){

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
		    Foto.RetornarAlbum(function (result)
		    {
		        itemArquivo.Situacao = $translate.instant('Foto_SubindoVideo');
		        var uploadVideo = new UploadVideo();
		        uploadVideo.itemArquivo = itemArquivo;
		        uploadVideo.MensagemProcessando = $translate.instant('Foto_ProcessandoVideo');
		        uploadVideo.callbackUpdate = function () {
		            $scope.$apply();
		        };
		        uploadVideo.ready(result[1], item.name, item, function(link, status)
		        {
		            var itemFoto = {
		                ImageMime: item.type, DataArquivo: moment.utc(item.lastModifiedDate).local().format("YYYY-MM-DDTHH:mm:ss"), CodigoGoogle: uploadVideo.videoId, Thumbnail: uploadVideo.thumbnail,
		                LinkGoogle: link
		            };
		            if (status == null) {
		                itemArquivo.Situacao = $translate.instant('Foto_Enviando');
		                Foto.SubirVideo(itemFoto, function (data) {
		                    itemArquivo.Situacao = $translate.instant('Foto_Sucesso');
		                    itemArquivo.CodigoSituacao = 1;
		                    SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'F', data.ItemRegistro.Identificador, true);

		                    if (!vm.continuaPesquisando)
		                        vm.ListaDados.push(data.ItemRegistro);
		                  
		                });
		            }
		            else
		            {
		                itemArquivo.Situacao = $translate.instant('Foto_ErroVideo').format(status);
		                itemArquivo.CodigoSituacao = 2;
		            }
		        });
		    })
		};

		vm.ProcessarImagem = function (item, itemArquivo) {
		    loadImage(
                item,
                function (img, dados) {
                    var imageURL = img.toDataURL();
                    var itemFoto = { Base64: imageURL, ImageMime: item.type, DataArquivo: moment.utc(item.lastModifiedDate).local().format("YYYY-MM-DDTHH:mm:ss") };
                    if (dados && dados.exif) {
                        var tags = dados.exif.getAll()
                        if (tags.GPSLatitude)
                        {
                            var arrPosicao = tags.GPSLatitude.split(',');
                            itemFoto.Latitude = vm.ConverterDegressDecimal(arrPosicao[0], arrPosicao[1], arrPosicao[2] == "NaN" ? 0 : arrPosicao[2], tags.GPSLatitudeRef)
                        }
                        if (tags.GPSLongitude) {
                            var arrPosicao = tags.GPSLongitude.split(',');
                            itemFoto.Longitude = vm.ConverterDegressDecimal(arrPosicao[0], arrPosicao[1], arrPosicao[2] == "NaN" ? 0 : arrPosicao[2], tags.GPSLongitudeRef)
                        }
                        if (tags.DateTimeOriginal)
                        {
                            itemFoto.DataArquivo = moment(tags.DateTimeOriginal, "YYYY:MM:DD HH:mm:ss").local().format("YYYY-MM-DDTHH:mm:ss");
                        }
                    }
                    itemArquivo.Situacao = $translate.instant('Foto_Enviando');

                    Foto.SubirImagem(itemFoto, function (data) {
                        itemArquivo.Situacao = $translate.instant('Foto_Sucesso');
                        itemArquivo.CodigoSituacao = 1;
                        SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'F', data.ItemRegistro.Identificador, true);

                        if (!vm.continuaPesquisando)
                            vm.ListaDados.push(data.ItemRegistro);
                      
                    });

                    
                },
                { maxWidth: 2048, maxHeight: 2048, canvas: true, orientation: true } // Options
            );
		};

		vm.ConverterDegressDecimal = function(degrees, minutes, seconds, direction) {
		    var dd = parseInt( degrees) + parseFloat( minutes) / 60 + parseFloat( seconds) / (60 * 60);

		    if (direction == "S" || direction == "W") {
		        dd = dd * -1;
		    } // Don't do anything for N or E
		    return dd;
		}

		vm.load = function () {
			vm.loading = true;
			Cidade.CarregarFoto(function (lista) {
			    vm.ListaCidades = lista;
                
			});
			Atracao.CarregarFoto(function (lista) {
			    vm.ListaAtracao = lista;
			    vm.filtro.Atracoes = lista;
			});
			Hotel.CarregarFoto(function (lista) {
			    vm.ListaHotel = lista;
			    vm.filtro.Hoteis = lista;
			});
			Refeicao.CarregarFoto(function (lista) {
			    vm.ListaRefeicao = lista;
			    vm.filtro.Refeicoes = lista;
			});
			Loja.CarregarFoto(function (lista) {
			    vm.ListaItemCompra = lista;
			});

			vm.CarregarDadosWebApi(vm.pageSize, vm.currentItem, vm.AjustarDadosPagina);

			
		};


		vm.delete = function (itemForDelete, callback) {
			vm.loading = true;
			Foto.delete({ id: itemForDelete.Identificador }, function (data) {
				callback(data);
				if (data.Sucesso) {
				    vm.currentItem--;

				    var itens =
				    $.grep(vm.ListaDados, function (e) { return e.Identificador == itemForDelete.Identificador; });
				    if (itens.length > 0)
				    {
				        var posicao = vm.ListaDados.indexOf(itens[0]);
				        vm.ListaDados.splice(posicao, 1);
				    }
					Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
				}
				else {
					var Mensagens = new Array();
					$(data.Mensagens).each(function (j, jitem) {
						Mensagens.push(jitem.Mensagem);
					});
				Error.showError('warning', $translate.instant("Alerta"), Mensagens.join("<br/>"), true);
				}
				vm.loading = false;
			},
			function (err) {
				$uibModalInstance.close();
				Error.showError('error', 'Ops!', $translate.instant("ErroExcluir"), true);
				vm.loading = false;
			})
		};

    
        

        vm.askDelete = function (itemForDelete) {
            // $uibModalInstance.close();
            $uibModal.open({
                templateUrl: 'modalDelete.html',
                controller: ['$uibModalInstance', 'item', vm.DeleteModalCtrl],
                controllerAs: 'vmDelete',
                resolve: {
                    item: function () { return itemForDelete; },
                }
            });
        };

        vm.DeleteModalCtrl = function ($uibModalInstance, itemForDelete) {
            var vmDelete = this;
            vmDelete.itemForDelete = itemForDelete;

            vmDelete.close = function () {
                $uibModalInstance.close();
            };

            vmDelete.back = function () {
                $uibModalInstance.close();
                vm.actionModal();
            };

            vmDelete.delete = function () {
                vm.delete(vmDelete.itemForDelete, function () {
                    $uibModalInstance.close();
                });
            };
        };

    

        vm.filtraDado = function () {

            vm.filtroAtualizacao = jQuery.extend({}, vm.filtro);                  

            vm.filtroAtualizacao.Hoteis = null;
            vm.filtroAtualizacao.Atracoes = null;
            vm.filtroAtualizacao.Refeicoes = null;
            if (vm.itemCidade && vm.itemCidade.Identificador)
                vm.filtroAtualizacao.IdentificadorCidade = vm.itemCidade.Identificador;
            else
                vm.filtroAtualizacao.IdentificadorCidade = null;

            if (vm.filtroAtualizacao.DataInicioDe) {
                if (typeof vm.filtroAtualizacao.DataInicioDe == "string") {
                    var date = Date.parse(vm.filtroAtualizacao.DataInicioDe);
                    if (!isNaN(date))
                        vm.filtroAtualizacao.DataInicioDe = $.datepicker.formatDate("yy-mm-ddT00:00:00", new Date(date));
                }
                else
                    vm.filtroAtualizacao.DataInicioDe = $.datepicker.formatDate("yy-mm-ddT00:00:00", vm.filtroAtualizacao.DataInicioDe);
            }

            if (vm.filtroAtualizacao.DataInicioAte) {
                if (typeof vm.filtroAtualizacao.DataInicioAte == "string") {
                    var date = Date.parse(vm.filtroAtualizacao.DataInicioAte);
                    if (!isNaN(date))
                        vm.filtroAtualizacao.DataInicioAte = $.datepicker.formatDate("yy-mm-ddT00:00:00", new Date(date));
                }
                else
                    vm.filtroAtualizacao.DataInicioAte = $.datepicker.formatDate("yy-mm-ddT00:00:00", vm.filtroAtualizacao.DataInicioAte);
            }

            vm.filtroAtualizacao.ListaHoteis = [];
            angular.forEach(vm.filtro.Hoteis, function (c) {
                if (c.Selecionado)
                    vm.filtroAtualizacao.ListaHoteis.push(c.Identificador);
            });

            vm.filtroAtualizacao.ListaAtracoes = [];
            angular.forEach(vm.filtro.Atracoes, function (c) {
                if (c.Selecionado)
                    vm.filtroAtualizacao.ListaAtracoes.push(c.Identificador);
            });

            vm.filtroAtualizacao.ListaRefeicoes = [];
            angular.forEach(vm.filtro.Refeicoes, function (c) {
                if (c.Selecionado)
                    vm.filtroAtualizacao.ListaRefeicoes.push(c.Identificador);
            });

            vm.pageSize = 30
            vm.currentItem = 0;
          

            vm.CarregarDadosWebApi(vm.pageSize, vm.currentItem, vm.AjustarDadosPagina);
        };



        vm.ProximaPagina = function () {
            if (vm.continuaPesquisando) {
                vm.enableScroll = false;
                vm.loading = true;
                vm.CarregarDadosWebApi(vm.pageSize, vm.currentItem, function (data) {

                    angular.forEach(data.Lista, function (c) {
                        vm.ListaDados.push(c);
                    });

                    vm.currentItem += data.Lista.length;
                    if (data.Lista.length == 0)
                        vm.continuaPesquisando = false
                    if (!$scope.$$phase) {
                        $scope.$apply();
                    }
                    vm.enableScroll = true;
                    vm.loading = false;
                });
            }
            
        };

        vm.AlterarSelecao = function (item) {
            item.Selecionado = !item.Selecionado;
        };

   
        vm.AjustarDadosPagina = function (data) {
            vm.ListaDados = data.Lista;
            vm.continuaPesquisando = true;
            vm.currentItem = data.Lista.length;
            vm.enableScroll = true;
            if (!$scope.$$phase) {
                $scope.$apply();
            }
        };

        vm.Idioma = function () {
            if (Auth && Auth.currentUser && Auth.currentUser.Cultura)
                return Auth.currentUser.Cultura.toLowerCase().substr(0, 2);
            else
                return "pt";
        };
//
        vm.CarregarDadosWebApi = function (pageSize, page, callback) {
            vm.loading = true;
            vm.filtroAtualizacao.Index = page;
            vm.filtroAtualizacao.Count = pageSize;

           
            Foto.list({ json: JSON.stringify(vm.filtroAtualizacao) }, function (data) {
                vm.loading = false;
                callback(data);
                             
                vm.loading = false;
            }, function (err) {
                vm.loading = false;
                Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), true);
                
                vm.loading = false;
            });
        };
	    //

        vm.RemoverUpload = function (item) {
            var posicao = vm.ListaArquivos.indexOf(item);
            vm.ListaArquivos.splice(posicao, 1);
        };

        vm.AbrirJanelaEdicao = function (itemFoto) {

            itemFoto.ListaAtracao = [];
            itemFoto.ListaRefeicao = [];
            itemFoto.ListaItemCompra = [];
            itemFoto.ListaHotel = [];
            angular.forEach(vm.ListaAtracao, function (c) {
                var item = jQuery.extend({}, c);
                item.Selecionado = $.grep(itemFoto.Atracoes, function (e) { return e.IdentificadorAtracao == item.Identificador && !e.DataExclusao; }).length > 0;
                itemFoto.ListaAtracao.push(item);
            });
            angular.forEach(vm.ListaRefeicao, function (c) {
                var item = jQuery.extend({}, c);
                item.Selecionado = $.grep(itemFoto.Refeicoes, function (e) { return e.IdentificadorRefeicao == item.Identificador && !e.DataExclusao; }).length > 0;
                itemFoto.ListaRefeicao.push(item);
            });
            angular.forEach(vm.ListaItemCompra, function (c) {
                var item = jQuery.extend({}, c);
                item.Selecionado = $.grep(itemFoto.ItensCompra, function (e) { return e.IdentificadorItemCompra == item.Identificador && !e.DataExclusao; }).length > 0;
                itemFoto.ListaItemCompra.push(item);
            });
            angular.forEach(vm.ListaHotel, function (c) {
                var item = jQuery.extend({}, c);
                item.Selecionado = $.grep(itemFoto.Hoteis, function (e) { return e.IdentificadorHotel == item.Identificador && !e.DataExclusao; }).length > 0;
                itemFoto.ListaHotel.push(item);
            });

            $uibModal.open({
                templateUrl: 'editaImagem.html',
                controller: ['$uibModalInstance', 'item', vm.EditModalCtrl],
                controllerAs: 'vmEdit',
                resolve: {
                    item: function () { return itemFoto; },
                }
            });
        };
      

        vm.EditModalCtrl = function ($uibModalInstance, item ) {
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
            vmEdit.edit = function (idToEdit) {
                $uibModalInstance.close();
                $state.go('FotoEdicao', { id: idToEdit, filtro: vm.filtroAtualizacao });
            };

            vmEdit.Excluir = function () {
                vm.askDelete(item);
                $uibModalInstance.close();
            };

            vmEdit.Idioma = function () {
                if (Auth && Auth.currentUser && Auth.currentUser.Cultura)
                    return Auth.currentUser.Cultura.toLowerCase().substr(0, 2);
                else
                    return "pt";
            };

            vmEdit.AlterarSelecaoAtracao = function (item)
            {
                item.Selecionado = !item.Selecionado;
                var itens =
                  $.grep(vmEdit.itemFoto.Atracoes, function (e) { return e.IdentificadorAtracao == item.Identificador && !e.DataExclusao });
                if (item.Selecionado && itens.length == 0)
                {
                    vmEdit.itemFoto.Atracoes.push({ IdentificadorAtracao: item.Identificador, ItemAtracao: item, DataAtualizacao: moment.utc(new Date()).format("YYYY-MM-DDTHH:mm:ss") });
                }
                else if (!item.Selecionado && itens.length > 0)
                {
                    //var posicao = vmEdit.itemFoto.Atracoes.indexOf(itens[0]);
                    //vmEdit.itemFoto.Atracoes.splice(posicao, 1);
                    itens[0].DataExclusao = moment.utc(new Date()).format("YYYY-MM-DDTHH:mm:ss");
                }
            };

            vmEdit.AlterarSelecaoHotel = function (item)
            {
                item.Selecionado = !item.Selecionado;
                var itens =
                  $.grep(vmEdit.itemFoto.Hoteis, function (e) { return e.IdentificadorHotel == item.Identificador && !e.DataExclusao });
                if (item.Selecionado && itens.length == 0) {
                    vmEdit.itemFoto.Hoteis.push({ IdentificadorHotel: item.Identificador, ItemHotel: item, DataAtualizacao: moment.utc(new Date()).format("YYYY-MM-DDTHH:mm:ss") });
                }
                else if (!item.Selecionado && itens.length > 0) {
                    //var posicao = vmEdit.itemFoto.Hoteis.indexOf(itens[0]);
                    //vmEdit.itemFoto.Hoteis.splice(posicao, 1);
                    itens[0].DataExclusao = moment.utc(new Date()).format("YYYY-MM-DDTHH:mm:ss");
                }
            };

            vmEdit.AlterarSelecaoRefeicao = function (item)
            {
                item.Selecionado = !item.Selecionado;
                var itens =
                  $.grep(vmEdit.itemFoto.Refeicoes, function (e) { return e.IdentificadorRefeicao == item.Identificador && !e.DataExclusao });
                if (item.Selecionado && itens.length == 0) {
                    vmEdit.itemFoto.Refeicoes.push({ IdentificadorRefeicao: item.Identificador, ItemRefeicao: item, DataAtualizacao: moment.utc(new Date()).format("YYYY-MM-DDTHH:mm:ss") });
                }
                else if (!item.Selecionado && itens.length > 0) {
                    //var posicao = vmEdit.itemFoto.Refeicoes.indexOf(itens[0]);
                    //vmEdit.itemFoto.Refeicoes.splice(posicao, 1);
                    itens[0].DataExclusao = moment.utc(new Date()).format("YYYY-MM-DDTHH:mm:ss");
                }
            };


            vmEdit.AlterarSelecaoItemCompra = function (item)
            {
                item.Selecionado = !item.Selecionado;
                var itens =
                 $.grep(vmEdit.itemFoto.ItensCompra, function (e) { return e.IdentificadorItemCompra == item.Identificador && !e.DataExclusao });
                if (item.Selecionado && itens.length == 0) {
                    vmEdit.itemFoto.ItensCompra.push({ IdentificadorItemCompra: item.Identificador, ItemItemCompra: item, DataAtualizacao: moment.utc(new Date()).format("YYYY-MM-DDTHH:mm:ss") });
                }
                else if (!item.Selecionado && itens.length > 0) {
                    itens[0].DataExclusao = moment.utc(new Date()).format("YYYY-MM-DDTHH:mm:ss");
                    //var posicao = vmEdit.itemFoto.ItensCompra.indexOf(itens[0]);
                    //vmEdit.itemFoto.ItensCompra.splice(posicao, 1);
                }
            };

            vmEdit.salvar = function () {
                if (vmEdit.itemFoto.Data) {
                    if (typeof vmEdit.itemFoto.Data == "string") {
                        var date = Date.parse(vmEdit.itemFoto.Data);
                        if (!isNaN(date))
                            vmEdit.itemFoto.Data = moment(new Date(date)).format("YYYY-MM-DDT")  ;
                    }
                    else
                        vmEdit.itemFoto.Data = moment(vmEdit.itemFoto.Data).format("YYYY-MM-DDT");
                    vmEdit.itemFoto.Data += (vmEdit.itemFoto.strHora) ? vmEdit.itemFoto.strHora : "00:00:00";
                }

                Foto.save(vmEdit.itemFoto, function (data) {
                    vmEdit.loading = false;
                    if (data.Sucesso) {
                        SignalR.ViagemAtualizada(Auth.currentUser.IdentificadorViagem, 'F', vmEdit.itemFoto.Identificador, false);

                        $uibModalInstance.close();
                    } else {
                        vmEdit.messages = data.Mensagens;
                        vmEdit.verificaCampoInvalido();
                    }
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
                    controller: ['$uibModalInstance', 'NgMap', '$timeout','$scope', 'item', vm.MapModalCtrl],
                    controllerAs: 'vmMapa',
                    resolve: {
                        item: function () { return vmEdit.itemFoto; },
                    }
                });
            };

        }

        vm.MapModalCtrl = function ($uibModalInstance,NgMap, $timeout,$scope, item) {
            var vmMapa = this;
            vmMapa.lat = 0;
            vmMapa.lng = 0;
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
                },500);
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

            vmMapa.ajustaPosicao = function(event) {
                var ll = event.latLng;
                vmMapa.itemMarcador = { Latitude: ll.lat(), Longitude: ll.lng() };

            };

            vmMapa.limparPosicao = function () {
                vmMapa.itemMarcador = {};
            };
        };
	}
}());
