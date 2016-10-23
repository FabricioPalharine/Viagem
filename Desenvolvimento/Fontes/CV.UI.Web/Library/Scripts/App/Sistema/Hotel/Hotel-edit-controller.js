(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('HotelEditCtrl',[ 'Error',  '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Cidade','Viagem','Hotel', HotelEditCtrl]);

	function HotelEditCtrl(  Error,  $state, $translate, $scope, Auth, $rootScope, $stateParams,Cidade,Viagem,Hotel) {
		var vm = this;
		vm.filtroConsulta = { };
		vm.itemHotel = { };
		vm.loading = false;
		vm.inclusao = false;
		vm.submitted = false;
		vm.messages = [];
		vm.edicao = false;
		vm.loggedUser = Auth.currentUser;
		vm.CamposInvalidos = {};
		vm.itemViagem = null;
		vm.ListaViagem = [];
		vm.itemCidade = null;
		vm.ListaCidade = [];

		vm.load = function () {
			vm.loading = true;
			var param = $stateParams;
			vm.filtroConsulta = param.filtro;
			Viagem.list({ json: JSON.stringify({}) }, function (data) {
				vm.ListaViagem=data.Lista;
			},
			function (err) {
				Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), false);
			});
			Cidade.list({ json: JSON.stringify({}) }, function (data) {
				vm.ListaCidade=data.Lista;
			},
			function (err) {
				Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), false);
			});

			if (param.id !== undefined && param.id !== '') {
				vm.edicao=true;
				Hotel.get({id: param.id }, function (data) {
					vm.itemHotel=data;
			if (data.IdentificadorViagem!=null)
				vm.itemViagem= {Identificador:data.IdentificadorViagem};
			if (data.IdentificadorCidade!=null)
				vm.itemCidade= {Identificador:data.IdentificadorCidade};

					vm.loading = false;
				}, function (err) {
					vm.loading = false;
					Error.showError('error', 'Ops!', $translate.instant('ErroRequisicao'), false);
				});
			}
			else
			{
				vm.inclusao=true;
				vm.loading = false
			};
		};
		vm.save = function (form) {
			vm.messages = [];
			vm.submitted = true;
			vm.CamposInvalidos = {};
			if (form.$valid) {
				if(vm.itemViagem!== null)
					vm.itemHotel.IdentificadorViagem= vm.itemViagem.Identificador;
				else
					vm.itemHotel.IdentificadorViagem= null;
				if(vm.itemCidade!== null)
					vm.itemHotel.IdentificadorCidade= vm.itemCidade.Identificador;
				else
					vm.itemHotel.IdentificadorCidade= null;
				if(vm.itemHotel.DataEntrada){
					if (typeof vm.itemHotel.DataEntrada == "string"){
						 var date = Date.parse(vm.itemHotel.DataEntrada);
						if (!isNaN(date))
							vm.itemHotel.DataEntrada=$.datepicker.formatDate("yy-mm-ddT00:00:00", new Date(date));
					}
					else
							vm.itemHotel.DataEntrada=$.datepicker.formatDate("yy-mm-ddT00:00:00", vm.itemHotel.DataEntrada);
				}
				if(vm.itemHotel.DataSaidia){
					if (typeof vm.itemHotel.DataSaidia == "string"){
						 var date = Date.parse(vm.itemHotel.DataSaidia);
						if (!isNaN(date))
							vm.itemHotel.DataSaidia=$.datepicker.formatDate("yy-mm-ddT00:00:00", new Date(date));
					}
					else
							vm.itemHotel.DataSaidia=$.datepicker.formatDate("yy-mm-ddT00:00:00", vm.itemHotel.DataSaidia);
				}
				if(vm.itemHotel.EntradaPrevista){
					if (typeof vm.itemHotel.EntradaPrevista == "string"){
						 var date = Date.parse(vm.itemHotel.EntradaPrevista);
						if (!isNaN(date))
							vm.itemHotel.EntradaPrevista=$.datepicker.formatDate("yy-mm-ddT00:00:00", new Date(date));
					}
					else
							vm.itemHotel.EntradaPrevista=$.datepicker.formatDate("yy-mm-ddT00:00:00", vm.itemHotel.EntradaPrevista);
				}
				if(vm.itemHotel.SaidaPrevista){
					if (typeof vm.itemHotel.SaidaPrevista == "string"){
						 var date = Date.parse(vm.itemHotel.SaidaPrevista);
						if (!isNaN(date))
							vm.itemHotel.SaidaPrevista=$.datepicker.formatDate("yy-mm-ddT00:00:00", new Date(date));
					}
					else
							vm.itemHotel.SaidaPrevista=$.datepicker.formatDate("yy-mm-ddT00:00:00", vm.itemHotel.SaidaPrevista);
				}

				Hotel.save(vm.itemHotel, function (data) {
					vm.loading = false;
					if (data.Sucesso) {
						Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
						$state.go('Hotel', { filtro: vm.filtroConsulta });
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
//
        vm.Idioma = function () {
            if (Auth && Auth.currentUser && Auth.currentUser.Cultura)
                return Auth.currentUser.Cultura.toLowerCase().substr(0, 2);
            else
                return "pt";
        };
//
        vm.verificaCampoInvalido = function () {
            vm.CamposInvalidos = {

            };
            //  var _retorno = false;
            $(vm.messages).each(function (i, item) {
                vm.checarAcessos[item.Campo] = true;
            });
        };
	}
}());
