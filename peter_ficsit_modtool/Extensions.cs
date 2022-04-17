using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pastel;
using peter_ficsit_api;
using peter_ficsit_api.GraphQL;

namespace peter_ficsit_api_implement
{
    public static class Extensions
    {
        public static string GetInfoString(this IGetMods_GetMods_Mods_LatestVersions? versions)
        {
            StringBuilder sb = new StringBuilder();

            if (versions.Alpha == null)
            {
                sb.Append("No Alpha Version");
            }
            else sb.Append((versions.Alpha.Version + " " + VersionStabilities.Alpha.ToString()).ColourStability(VersionStabilities.Alpha));

            sb.Append(" - ");
            
            if (versions.Beta == null)
            {
                sb.Append("No Beta Version");
            }
            else sb.Append((versions.Beta.Version + " " + VersionStabilities.Beta.ToString()).ColourStability(VersionStabilities.Beta));

            sb.Append(" - ");

            if (versions.Release == null)
            {
                sb.Append("No Release Version");
            }
            else sb.Append((versions.Release.Version + " " + VersionStabilities.Release.ToString()).ColourStability(VersionStabilities.Release));

            return sb.ToString();
        }

        public static string GetInfoString(this ISearchMods_GetMods_Mods_LatestVersions? versions)
        {
            StringBuilder sb = new StringBuilder();

            if (versions.Alpha == null)
            {
                sb.Append("No Alpha Version");
            }
            else sb.Append((versions.Alpha.Version + " " + VersionStabilities.Alpha).ColourStability(VersionStabilities.Alpha));

            sb.Append(" - ");

            if (versions.Beta == null)
            {
                sb.Append("No Beta Version");
            }
            else sb.Append((versions.Beta.Version + " " + VersionStabilities.Beta).ColourStability(VersionStabilities.Beta));

            sb.Append(" - ");

            if (versions.Release == null)
            {
                sb.Append("No Release Version");
            }
            else sb.Append((versions.Release.Version + " " + VersionStabilities.Release).ColourStability(VersionStabilities.Release));

            return sb.ToString();
        }

        public static string GetInfoString(this IGetMod_Mod_LatestVersions? versions)
        {
            StringBuilder sb = new StringBuilder();

            if (versions.Alpha == null)
            {
                sb.AppendLine("No Alpha Version");
            }
            else
            {
                sb.Append((versions.Alpha.Version + " " + VersionStabilities.Alpha.ToString()).ColourStability(VersionStabilities.Alpha));
                sb.AppendLine($" - SML: {versions.Alpha.Sml_version} - Created: {versions.Alpha.Created_at}");
                sb.AppendLine(StaticOptions.SiteURL() + versions.Alpha.Link);
            }

            sb.AppendLine("");

            if (versions.Beta == null)
            {
                sb.AppendLine("No Beta Version");
            }
            else
            {
                sb.Append((versions.Beta.Version + " " + VersionStabilities.Beta.ToString()).ColourStability(VersionStabilities.Beta));
                sb.AppendLine($" - SML: {versions.Beta.Sml_version} - Created: {versions.Beta.Created_at}");
                sb.AppendLine(StaticOptions.SiteURL() + versions.Beta.Link);
            }

            sb.AppendLine("");

            if (versions.Release == null)
            {
                sb.AppendLine("No Release Version");
            }
            else
            {
                sb.Append((versions.Release.Version + " " + VersionStabilities.Release.ToString()).ColourStability(VersionStabilities.Release));
                sb.AppendLine($" - SML: {versions.Release.Sml_version} - Created: {versions.Release.Created_at}");
                sb.AppendLine(StaticOptions.SiteURL() + versions.Release.Link);
            }

            return sb.ToString();
        }

        public static string ColourStability(this string str, VersionStabilities stabilities)
        {
            switch (stabilities)
            {
                case VersionStabilities.Alpha:
                    return str.Pastel(Color.OrangeRed);
                    break;
                case VersionStabilities.Beta:
                    return str.Pastel(Color.Orange);
                    break;
                case VersionStabilities.Release:
                    return str.Pastel(Color.LimeGreen);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stabilities), stabilities, null);
            }
        }
    }
}
