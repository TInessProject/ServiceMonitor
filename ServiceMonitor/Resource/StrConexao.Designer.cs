﻿//------------------------------------------------------------------------------
// <auto-generated>
//     O código foi gerado por uma ferramenta.
//     Versão de Tempo de Execução:4.0.30319.42000
//
//     As alterações ao arquivo poderão causar comportamento incorreto e serão perdidas se
//     o código for gerado novamente.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ServiceMonitor.Resource {
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
    internal class StrConexao {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal StrConexao() {
        }
        
        /// <summary>
        ///   Retorna a instância de ResourceManager armazenada em cache usada por essa classe.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ServiceMonitor.Resource.StrConexao", typeof(StrConexao).Assembly);
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
        ///   Consulta uma cadeia de caracteres localizada semelhante a Data Source=sqlcardio.unimed;Initial Catalog=SynergiusDesenvolvimento;Persist Security Info=True;Connection Timeout=200;User ID=Synergius;Password=syn3rgius.
        /// </summary>
        internal static string Desenvolvimento {
            get {
                return ResourceManager.GetString("Desenvolvimento", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Data Source=sqlteste.unimed;Initial Catalog=SynergiusHomologacao;Persist Security Info=True;Connection Timeout=200; User ID=Synergius;Password=syn3rgius.
        /// </summary>
        internal static string Homologacao {
            get {
                return ResourceManager.GetString("Homologacao", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Data Source=sqlcardio.unimed;Initial Catalog=Synergius;Persist Security Info=True;User ID=Synergius;Password=syn3rgius.
        /// </summary>
        internal static string Producao {
            get {
                return ResourceManager.GetString("Producao", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Data Source=Synergius/synergius123@srv07-scan/myservice:dedicated/syneprod.unimed.local;.
        /// </summary>
        internal static string ProducaoOracle {
            get {
                return ResourceManager.GetString("ProducaoOracle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Data Source=sqlteste.unimed;Initial Catalog=SynergiusTreinamento;Persist Security Info=True;Connection Timeout=200; User ID=Synergius;Password=syn3rgius.
        /// </summary>
        internal static string Treinamento {
            get {
                return ResourceManager.GetString("Treinamento", resourceCulture);
            }
        }
    }
}
