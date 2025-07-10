using R2API;
using System;
using System.Collections.Generic;
using System.Text;

namespace SicarianInfiltrator
{
    public class Language
    {
        public static void AddLanguageToken(string token, string output, string language)
        {
            Dictionary<string, string> keyValuePairs = RoR2.Language.languagesByName.ContainsKey(language) ? RoR2.Language.languagesByName[language].stringsByToken : null;
            if (keyValuePairs == null) return;
            if (keyValuePairs.ContainsKey(token))
            {
                keyValuePairs[token] = output;
            }
            else
            {
                keyValuePairs.Add(token, output);
            }
        }
        public static void AddLanguageToken(string token, string output)
        {
            AddLanguageToken(token, output, "en");
        }
    }
}
