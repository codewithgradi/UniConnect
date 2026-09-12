// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Design;
// using Microsoft.Extensions.Configuration;
// using System;
// using System.IO;

// namespace Infrastructure.Persistence;

// public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
// {
//     public ApplicationDbContext CreateDbContext(string[] args)
//     {
//         // Traverse upward to locate the root .env file automatically
//         var currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());
//         string envFilePath = null;

//         while (currentDir != null)
//         {
//             var testPath = Path.Combine(currentDir.FullName, ".env");
//             if (File.Exists(testPath))
//             {
//                 envFilePath = testPath;
//                 break;
//             }
//             currentDir = currentDir.Parent;
//         }

//         string connectionString = null;

//         if (envFilePath != null && File.Exists(envFilePath))
//         {
//             foreach (var line in File.ReadAllLines(envFilePath))
//             {
//                 var parts = line.Split('=', 2, StringSplitOptions.RemoveEmptyEntries);
//                 if (parts.Length == 2)
//                 {
//                     var key = parts[0].Trim();
//                     var val = parts[1].Trim().Trim('"').Trim('\'');

//                     if (key.Equals("ConnectionStrings__ProdDB", StringComparison.OrdinalIgnoreCase) ||
//                         key.Equals("ProdDB", StringComparison.OrdinalIgnoreCase))
//                     {
//                         connectionString = val;
//                         break;
//                     }
//                 }
//             }
//         }

//         // Fallback to machine environment variables if needed
//         connectionString ??= Environment.GetEnvironmentVariable("ConnectionStrings__ProdDB")
//                           ?? Environment.GetEnvironmentVariable("ProdDB");

//         if (string.IsNullOrEmpty(connectionString) || connectionString.Equals("placeholder", StringComparison.OrdinalIgnoreCase))
//         {
//             throw new InvalidOperationException($"Could not find a valid ProdDB connection string. Checked upward for .env and found: {envFilePath ?? "None"}");
//         }

//         var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
//         builder.UseNpgsql(connectionString);

//         var context = new ApplicationDbContext(builder.Options);

//         // Completely wipe the schema so EF Core can build it cleanly from scratch
//         try
//         {
//             context.Database.ExecuteSqlRaw("DROP SCHEMA public CASCADE; CREATE SCHEMA public;");
//         }
//         catch
//         {
//             // Suppress if it fails on an empty database
//         }

//         return context;
//     }
// }