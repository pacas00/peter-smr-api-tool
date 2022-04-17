using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pastel;
using peter_ficsit_api.GraphQL;
using StrawberryShake;
using static peter_ficsit_api_implement.Extensions;

namespace peter_ficsit_api_implement
{
    public class ApiSearchImplem
    {
        public static void DoModSearch(string SearchTerm, int SearchLimit = 50)
        {
            var client = ClientBuilder.GetClient(false);

            var result = client.SearchMods.ExecuteAsync(SearchTerm, SearchLimit).GetAwaiter().GetResult();
            result.EnsureNoErrors();
            foreach (var session in result.Data.GetMods.Mods)
            {
                Console.WriteLine($"{session.Name} - {session.Id} - {session.Short_description}");
                Console.WriteLine($"V: {session.LatestVersions.GetInfoString()}");
                Console.WriteLine();
            }
        }


        public static void GetSMLVersions(int limit = 50, int offset = 0)
        {
            var client = ClientBuilder.GetClient(false);

            var result = client.GetSMLVersions.ExecuteAsync(limit, offset).GetAwaiter().GetResult();
            result.EnsureNoErrors();

            foreach (var session in result.Data.GetSMLVersions.Sml_versions)
            {
                Console.WriteLine($"{session.Id} - {session.Version} - {session.Satisfactory_version} - {session.Stability} - {session.Date}".ColourStability(session.Stability));
                Console.WriteLine($"DL: {session.Link}");
                Console.WriteLine(session.Changelog);
                Console.WriteLine();
            }
        }

        public static void ListMods(int limit = 10, int page = 0, string? SearchText = null)
        {
            var client = ClientBuilder.GetClient(false);

            var result = client.GetMods.ExecuteAsync(page, limit, SearchText, Order.Desc, ModFields.UpdatedAt).GetAwaiter().GetResult();
            result.EnsureNoErrors();

            foreach (var session in result.Data.GetMods.Mods)
            {
                Console.WriteLine($"{session.Id} - {session.Mod_reference} - {session.Name} - {session.Short_description}");
                Console.WriteLine($"V: {session.LatestVersions.GetInfoString()}");
                Console.WriteLine($"DLs: {session.Downloads} - Views: {session.Views}");
                Console.WriteLine();
            }
        }

        public static void GetMod(string ModID)
        {
            var client = ClientBuilder.GetClient(false);

            var result = client.GetMod.ExecuteAsync(ModID).GetAwaiter().GetResult();
            result.EnsureNoErrors();

            var session = result.Data.Mod;
            {
                Console.WriteLine($"{session.Id} - {session.Mod_reference} - {session.Name} - {session.Short_description}");
                Console.WriteLine($"DLs: {session.Downloads} - Views: {session.Views}");
                Console.WriteLine();
                Console.WriteLine($"{session.Full_description}");
                Console.WriteLine();
                Console.WriteLine($"{session.LatestVersions.GetInfoString()}");
                Console.WriteLine();
            }
        }

        public static void GetModVersion(string VersionID)
        {
            var client = ClientBuilder.GetClient(false);

            var result = client.GetModVersion.ExecuteAsync(VersionID).GetAwaiter().GetResult();
            result.EnsureNoErrors();

            var session = result.Data.GetVersion;
            var mod = result.Data.GetVersion.Mod;
            {
                Console.WriteLine($"{mod.Id} - {mod.Mod_reference} - {mod.Name}");
                Console.WriteLine($"{session.Id} - SML: {session.Sml_version} - Stability: {session.Stability.ToString().ColourStability(session.Stability)}");
                Console.WriteLine($"DLs: {session.Downloads} - Created: {session.Created_at}");
                Console.WriteLine();
                Console.WriteLine($"{session.Changelog}");
                Console.WriteLine();
                Console.WriteLine($"{session.Link}");
                Console.WriteLine();
            }
        }
    }
}
