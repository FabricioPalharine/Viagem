(function () {
    'use strict';

    /**
     * @ngdoc object
     * @name account.controller:UserEditCtrl
     *
     * @description
     *
     */
    angular
      .module('home')
      .controller('AlteraSenhaCtrl', ['$stateParams', 'Auth', '$scope','Error', '$translate', AlteraSenhaCtrl]);

    function AlteraSenhaCtrl( $stateParams, Auth, $scope,Error, $translate) {

        //VARIAVEIS - INICIO
        var vm = this;
        vm.ctrlName = 'AlteraSenhaCtrl';
        vm.senha = '';
        vm.novasenha=''
        vm.confirmarsenha = '';
        vm.messages = [];
        vm.CamposInvalidos = {
            Senha: false,
            NovaSenha: false,
            ConfirmarSenha: false
        };
        vm.loggedUser = Auth.currentUser;
        vm.loading = false;

        vm.save = function (form) {
            this.submitted = true;

            if (form.$valid) {
                vm.loading = true;

                vm.messages = [];
                vm.CamposInvalidos = {
                    
                };

                Auth.AlterarSenha({
                    login: vm.loggedUser.LoginUsuario,
                    senha: vm.senha,
                    novasenha: vm.novasenha,
                    confirmarsenha: vm.confirmarsenha
                }).then(function (data) {
                    vm.loading = false;
                    if (data.Sucesso) {
                        Error.showError('success', $translate.instant("Sucesso"), data.Mensagens[0].Mensagem, true);
                        
                    } else {
                        vm.messages = data.Mensagens;
                        vm.verificaCampoInvalido();
                    }


                }).catch(function (err) {
                    vm.loading = false;
                    Error.showError('error', 'Ops!', $translate.instant("ErroSalvar"), true);
                    console.log(err);
                });
            }
        };

        vm.verificaCampoInvalido = function () {
            vm.CamposInvalidos = {              
            };
            //  var _retorno = false;
            $(vm.messages).each(function (i, item) {
               
                vm.CamposInvalidos[item.Campo] = true;
               
                
            });
        };
    }


}()
);