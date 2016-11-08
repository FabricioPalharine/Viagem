﻿using CV.Model;
using CV.Model.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CV.UI.Web.Controllers.WebAPI
{
    public class DominioController : BaseApiController
    {
        // GET api/<controller>
        [ActionName("CarregaMoedas")]
        [HttpGet]
        public IEnumerable<ItemLista> CarregarMoedas()
        {
            List<ItemLista> lista = new List<ItemLista>();
            foreach (var enumerador in Enum.GetValues(typeof(enumMoeda)))
            {
                var item = new ItemLista() { Codigo = Convert.ToInt32(enumerador).ToString(), Descricao = ((enumMoeda)enumerador).Descricao() };
                lista.Add(item);
            }
            return lista;
        }

        
    }
}