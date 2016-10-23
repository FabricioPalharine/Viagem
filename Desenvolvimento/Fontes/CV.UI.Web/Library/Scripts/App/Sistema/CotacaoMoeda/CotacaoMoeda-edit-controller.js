(function () {
	'use strict';
	angular
		.module('Sistema')
		.controller('CotacaoMoedaEditCtrl',[ 'Error',  '$state', '$translate', '$scope', 'Auth', '$rootScope', '$stateParams', 'Viagem','CotacaoMoeda', CotacaoMoedaEditCtrl]);

	function CotacaoMoedaEditCtrl(  Error,  $state, $translate, $scope, Auth, $rootScope, $stateParams,Viagem,CotacaoMoeda) {
		var vm = this;
		vm.filtroConsulta = { };
		vm.itemCotacaoMoeda = { };
		vm.loading = false;
		vm.inclusao = false;
		vm.submitted = false;
		vm.messages = [];
		vm.edicao = false;
		vm.loggedUser = Auth.currentUser;
		vm.CamposInvalidos = {};
		vm.itemViagem = null;
		vm.ListaViagem = [];

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

			if (param.id !== undefined && param.id !== '') {
				vm.edicao=true;
				CotacaoMoeda.get({id: param.id }, function (data) {
					vm.itemCotacaoMoeda=data;
			if (data.IdentificadorViagem!=null)
				vm.itemViagem= {Identificador:data.IdentificadorViagem};

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
				if(vm.itemCotacaoMoeda.DataCotacao){
					if (typeof vm.itemCotacaoMoeda.DataCotacao == "string"){
						 var date = Date.parse(vm.itemCotacaoMoeda.DataCotacao);
						if (!isNaN(date))
							vm.itemCotacaoMoeda.DataCotacao=$.datepicker.formatDate("yy-mm-ddT00:00:00", new Date(date));
					}
					else
							vm.itemCotacaoMoeda.DataCotacao=$.datepicker.formatDate("yy-mm-ddT00:00:00", vm.itemCotacaoMoeda.DataCotacao);
				}
				if(vm.itemViagem!== null)
					vm.itemCotacaoMoeda.IdentificadorViagem= vm.itemViagem.Identificador;
				else
					vm.itemCotacaoMoeda.IdentificadorViagem= null;

				CotacaoMoeda.save(vm.itemCotacaoMoeda, function (data) {
					vm.loading = false;
					if (data.Sucesso) {
						Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
						$state.go('CotacaoMoeda', { filtro: vm.filtroConsulta });
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
