using G1ANT.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Please remember to refresh G1ANT.Language.dll in references

namespace G1ANT.Addon.OpenAI
{
    [Addon(Name = "OpenAI", Tooltip = "Addon to integrate with OpenAI")]
    [Copyright(Author = "G1ANT LTD", Copyright = "G1ANT LTD", Email = "hi@g1ant.com", Website = "www.g1ant.com")]
    [License(Type = "LGPL", ResourceName = "License.txt")]
    [CommandGroup(Name = "openai", Tooltip = "Commands to work with OpenAI models")]
    public class OpenAIAddon : Language.Addon
    {

        public override void Check()
        {
            base.Check();
        }

        public override void LoadDlls()
        {
            base.LoadDlls();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}