﻿//------------------------------------------------------------------------------
// <auto-generated>
//     O código foi gerado por uma ferramenta.
//     Versão de Tempo de Execução:4.0.30319.42000
//
//     As alterações ao arquivo poderão causar comportamento incorreto e serão perdidas se
//     o código for gerado novamente.
// </auto-generated>
//------------------------------------------------------------------------------

namespace M0.Resource {
    using System;
    
    
    /// <summary>
    ///   Uma classe de recurso de tipo de alta segurança, para pesquisar cadeias de caracteres localizadas etc.
    /// </summary>
    // Essa classe foi gerada automaticamente pela classe StronglyTypedResourceBuilder
    // através de uma ferramenta como ResGen ou Visual Studio.
    // Para adicionar ou remover um associado, edite o arquivo .ResX e execute ResGen novamente
    // com a opção /str, ou recrie o projeto do VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class WS {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal WS() {
        }
        
        /// <summary>
        ///   Retorna a instância de ResourceManager armazenada em cache usada por essa classe.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("M0.Resource.WS", typeof(WS).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Substitui a propriedade CurrentUICulture do thread atual para todas as
        ///   pesquisas de recursos que usam essa classe de recurso de tipo de alta segurança.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a select * from M0.Servico where Codigo = @Servico.
        /// </summary>
        internal static string BuscarServico {
            get {
                return ResourceManager.GetString("BuscarServico", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a select * from M0.ServicoEvento where Situacao = &apos;A&apos; and Servico = @Servico order by Ordem.
        /// </summary>
        internal static string BuscarServicoEvento {
            get {
                return ResourceManager.GetString("BuscarServicoEvento", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a select * from M0.ServicoEventoAcao where Situacao = &apos;A&apos;.
        /// </summary>
        internal static string BuscarServicoEventoAcao {
            get {
                return ResourceManager.GetString("BuscarServicoEventoAcao", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a select * from M0.ServicoEventoCondicao where Situacao = &apos;A&apos;.
        /// </summary>
        internal static string BuscarServicoEventoCondicao {
            get {
                return ResourceManager.GetString("BuscarServicoEventoCondicao", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a select Codigo, Nome, Json, Permissao, Padrao, Favorito from M0.ServicoMemorizar where Servico = @Servico.
        /// </summary>
        internal static string BuscarServicoMemorizar {
            get {
                return ResourceManager.GetString("BuscarServicoMemorizar", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a select  
        ///(select SV.Nome from M0.Servico_Parametro SPV inner join M0.Servico SV on SPV.Servico = SV.Codigo  where SPV.Servico = SP.servico and SPV.Nome = SP.Link_Botao) as Link_Servico_Nome, SP.Link_Nome, SP.Link_Botao,
        ///SP.Codigo, SP.Servico, SP.Nome, SP.Obrigatorio, SP.Descricao, SP.Tipo, SP.Dominio, SP.Criacao_Usuario, SP.Visivel, SP.Servico_Parametro_Pai, 
        ///SP.Chave, SP.Pesquisa, SP.Nome_Exibicao, SP.Somente_Leitura, SP.Tamanho, coalesce(SP.Tipo_Entrada, &apos;&apos;) as Tipo_Entrada, SP.Valor_Padrao_Servico, SP [o restante da cadeia de caracteres foi truncado]&quot;;.
        /// </summary>
        internal static string BuscarServicoParametro {
            get {
                return ResourceManager.GetString("BuscarServicoParametro", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a select  
        ///(select SV.Nome from M0.Servico_Parametro SPV inner join M0.Servico SV on SPV.Servico = SV.Codigo  where SPV.Servico = SP.servico and SPV.Nome = SP.Link_Botao) as Link_Servico_Nome, SP.Link_Nome, SP.Link_Botao,
        ///SP.Codigo, SP.Servico, SP.Nome, SP.Obrigatorio, SP.Descricao, SP.Tipo, SP.Dominio, SP.Criacao_Usuario, SP.Visivel, SP.Servico_Parametro_Pai, 
        ///SP.Chave, SP.Pesquisa, SP.Nome_Exibicao, SP.Somente_Leitura, SP.Tamanho, coalesce(SP.Tipo_Entrada, &apos;&apos;) as Tipo_Entrada, SP.Valor_Padrao_Servico, SP [o restante da cadeia de caracteres foi truncado]&quot;;.
        /// </summary>
        internal static string BuscarServicoParametroCodigo {
            get {
                return ResourceManager.GetString("BuscarServicoParametroCodigo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a SELECT 
        ///ser.container_name as ContainerName, 
        ///SP.Container, 
        ///SP.Container_Largura as ContainerWidth,
        ///SP.Container_Altura as ContainerHeight,
        ///json.type, 
        ///SP.Nome as name, --json.name, 
        ///case when SP.Nome_Exibicao is null then json.label else SP.Nome_Exibicao end as label, 
        ///json.class, 
        ///case when tse.Tipo_Exibicao is null or json.action in(&apos;save&apos;, &apos;delete&apos;, &apos;find&apos;, &apos;update&apos;) then json.action else LOWER(tse.Tipo_Exibicao) end as action,
        ///json.element, 
        ///case when SP.Icone is not null and SP.Icone &lt;&gt; &apos;&apos; [o restante da cadeia de caracteres foi truncado]&quot;;.
        /// </summary>
        internal static string BuscarTipoServicoBoteosJSON {
            get {
                return ResourceManager.GetString("BuscarTipoServicoBoteosJSON", resourceCulture);
            }
        }
    }
}
