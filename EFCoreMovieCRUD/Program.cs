using EFCoreMovieCRUD.Data;
using EFCoreMovieCRUD.Models;
using EFCoreMovieCRUD.Services;
using Microsoft.EntityFrameworkCore;


using var context = new FilmDbContext();            // Ensure database is created - this handles the initial setup

// INIT SERVICE
var movieService = new MovieService(context);
var genreService = new GenreService(context);
var directorService = new DirectorService(context);


try
{
    await ApplyMigrationsAsync(context);
    await GenreOperation(genreService);
    await DirectorOperation(directorService);
    await MovieOperation(movService: movieService, dirService: directorService, gnrService: genreService);

}
catch (System.Exception ex)
{
    Console.WriteLine($"\nError occurred: {ex.Message}");

}

static async Task ApplyMigrationsAsync(FilmDbContext context)
{

    // DELETES the 'FilmDatabase.db' file if it exists
    await context.Database.EnsureDeletedAsync();

    Console.WriteLine("Applying database migrations...");

    // Apply any pending migrations
    await context.Database.MigrateAsync();

    Console.WriteLine("Database migrations applied successfully!\n");
}


static async Task GenreOperation(GenreService gnrService)
{
    Console.WriteLine("\n----------------------------------------");
    Console.WriteLine("[1]. MOVIES & RELATIONSHIPS DEMO");
    Console.WriteLine("----------------------------------------");

    Console.WriteLine("\n[1]. Adding a new genre...");

    var scifi = await gnrService.CreateGenreAsync(new Genre { Name = "Sci-Fi" });
    Console.WriteLine($"Genre added -- {scifi.Id} -- {scifi.Name}");

    var action = await gnrService.CreateGenreAsync(new Genre { Name = "Action" });
    Console.WriteLine($"Genre added -- {action.Id} -- {action.Name}");

    var drama = await gnrService.CreateGenreAsync(new Genre { Name = "Drama" });
    Console.WriteLine($"Genre added -- {drama.Id} -- {drama.Name}");

    var horror = await gnrService.CreateGenreAsync(new Genre { Name = "Horror" });
    Console.WriteLine($"Genre added -- {horror.Id} -- {horror.Name}");

    Console.WriteLine("\n[2]. Get ALL genre...");
    var allGenre = await gnrService.GetAllGenreAsync();

    Console.WriteLine("HERE IT IS : ");
    foreach (var item in allGenre)
    {
        Console.WriteLine($"{item.Id} -- {item.Name}");
    }

    // READ: Get specific genre by ID
    Console.WriteLine("\nGet Genre BY ID...");
    if (allGenre.Any())
    {
        var firstGenre = allGenre.First();
        var genreById = await gnrService.GetGenreByIdAsync(firstGenre.Id);
        if (genreById != null)
        {
            Console.WriteLine($"\nFIRST ON THE LIST : {firstGenre.Id} -- {firstGenre.Name}");
            Console.WriteLine($"Retrieved Genre by ID {genreById.Id} : {genreById.Name}");

        }
    }

    Console.WriteLine("[3]. Delete a genre");
    var isDeleted = await gnrService.DeleteGenreAsync(horror.Id);

    if (isDeleted)
    {
        Console.WriteLine($"Deleted Genre ID {horror.Id} ('{horror.Name}')");
    }

    else
    {
        Console.WriteLine($"❌ Failed to delete Genre ID {horror.Id}");
    }
    // 3. Verify it is really gone from SQLite
    var check = await gnrService.GetGenreByIdAsync(horror.Id);
    if (check == null)
    {
        Console.WriteLine("Confirmed: Genre no longer exists in database!");
    }

    Console.WriteLine("\n[4]. Update a genre...");

    allGenre = await gnrService.GetAllGenreAsync();

    if (allGenre.Any())
    {
        var genreToUpdate = allGenre.Last();
        var originalName = genreToUpdate.Name;

        // Change the name (e.g., from "Drama" -> "Psychological Drama")
        genreToUpdate.Name = "Psychological " + originalName;

        var updatedGenre = await gnrService.UpdateGenreAsync(genreToUpdate.Id, genreToUpdate);
        if (updatedGenre != null)
        {
            Console.WriteLine($"    Updated Genre ID {updatedGenre.Id}:");
            Console.WriteLine($"    OLD Name: {originalName} ->  NEW Name {updatedGenre.Name}");
        }

    }



}

static async Task DirectorOperation(DirectorService dirService)
{
    Console.WriteLine("\n----------------------------------------");
    Console.WriteLine("[2]. DIRECTORS DEMO");
    Console.WriteLine("----------------------------------------");

    // 1. Create Directors
    Console.WriteLine("\n[1]. Adding a new Director...");
    await dirService.CreateDirectorAsync(new Director { Name = "Christopher Nolan" });

    await dirService.CreateDirectorAsync(new Director { Name = "Quentin Tarantino" });

    await dirService.CreateDirectorAsync(new Director { Name = "Jono Watu Kewalik" });

    // 2. Read All Directors
    Console.WriteLine("\n[2]. Get all director...");
    var allDirectors = await dirService.GetAllDirectorAsync();

    Console.WriteLine("ALL DIRECTORS:");
    foreach (var dir in allDirectors)
    {
        Console.WriteLine($"ID: {dir.Id} -- Name: {dir.Name}");
    }

    // 3. Read Director by ID
    var firstDir = allDirectors.First();
    var found = await dirService.GetDirectorByIdAsync(firstDir.Id);
    if (found != null)
    {
        Console.WriteLine($"Retrieved Director by ID: {found.Id} - {found.Name}");
    }


    var jono = allDirectors.Last();
    var originalName = jono.Name;
    jono.Name = "John Flipped Stone";

    var updated = await dirService.UpdateDirectorAsync(jono.Id, jono);
    if (updated != null)
    {
        Console.WriteLine($"Updated Director ID {updated.Id}: '{originalName}' → '{updated.Name}'");
    }

    Console.WriteLine("DELETING DIRECTOR");
    // DELETE "John Flipped Stone"
    var isDeleted = await dirService.DeleteDirectorAsync(jono.Id);

    if (isDeleted)
    {
        Console.WriteLine($"Successfully deleted Director ID {jono.Id} ('{jono.Name}')");
    }

    // 4. VERIFY HE IS GONE FROM DATABASE
    var check = await dirService.GetDirectorByIdAsync(jono.Id);
    if (check == null)
    {
        Console.WriteLine("cHECKING: John Flipped Stone no longer exists in database!");
    }

}

static async Task MovieOperation(MovieService movService, DirectorService dirService, GenreService gnrService)
{
    Console.WriteLine("\n----------------------------------------");
    Console.WriteLine("[3]. MOVIES & RELATIONSHIPS DEMO");
    Console.WriteLine("----------------------------------------");

    // 1. Fetch existing Directors & Genres to link them
    var directors = await dirService.GetAllDirectorAsync();
    var genres = await gnrService.GetAllGenreAsync();

    var nolan = directors.First(d => d.Name == "Christopher Nolan");
    var tarantino = directors.First(d => d.Name == "Quentin Tarantino");

    var sciFi = genres.First(g => g.Name == "Sci-Fi");
    var action = genres.First(g => g.Name == "Action");
    var drama = genres.First(g => g.Name == "Psychological Drama");

    Console.WriteLine(" Create Movies ");

    // 2. Create Movies with linked Director & Genres
    var inception = new Movie
    {
        Title = "Inception",
        ReleaseYear = 2010,
        DirectorId = nolan.Id,
        Genres = new List<Genre> { sciFi, action } // Many-to-Many!
    };
    await movService.CreateMovieAsync(inception);

    var interstellar = new Movie
    {
        Title = "Interstellar",
        ReleaseYear = 2014,
        DirectorId = nolan.Id,
        Genres = new List<Genre> { sciFi, drama }
    };
    await movService.CreateMovieAsync(interstellar);

    var pulpFiction = new Movie
    {
        Title = "Pulp Fiction",
        ReleaseYear = 1994,
        DirectorId = tarantino.Id,
        Genres = new List<Genre> { drama, action }
    };
    await movService.CreateMovieAsync(pulpFiction);

    var pulpyFiction = new Movie
    {
        Title = "Pulpy Fiction Orange",
        ReleaseYear = 2026,
        DirectorId = tarantino.Id,
        Genres = new List<Genre> { drama, action }
    };
    await movService.CreateMovieAsync(pulpyFiction);

    

    // 3. Read All Movies with details
    Console.WriteLine("\nALL MOVIES WITH DETAILS:");
    var allMovies = await movService.GetAllMovieAsync();

    foreach (var movie in allMovies)
    {
        var genreList = string.Join(", ", movie.Genres.Select(g => g.Name));
        Console.WriteLine($" {movie.Title} ({movie.ReleaseYear})");
        Console.WriteLine($"   Director: {movie.Director?.Name ?? "N/A"}");
        Console.WriteLine($"   Genres:   {genreList}");
        Console.WriteLine();
    }


    // ========================================================
    // GET MOVIE BY ID DEMO
    // ========================================================
    Console.WriteLine("\n Get Movie by ID...");

    // Fetch Inception by its ID (inception.Id)
    var movieById = await movService.GetMovieByIdAsync(inception.Id);

    if (movieById != null)
    {
        var genreNames = string.Join(", ", movieById.Genres.Select(g => g.Name));

        Console.WriteLine($" Retrieved Movie by ID {movieById.Id}:");
        Console.WriteLine($"    Title:    {movieById.Title} ({movieById.ReleaseYear})");
        Console.WriteLine($"    Director: {movieById.Director?.Name ?? "Unknown"}");
        Console.WriteLine($"    Genres:   {genreNames}");
    }

    // 4. Filter Movies by Genre (Sci-Fi)
    Console.WriteLine($"FILTER: Movies in Genre '{sciFi.Name}':");

    var sciFiMovies = await movService.GetMovieByGenreAsync(sciFi.Id);
    foreach (var m in sciFiMovies)
    {
        Console.WriteLine($" - {m.Title}");
    }

    // 5. Filter Movies by Director (Christopher Nolan)
    Console.WriteLine($"\nFILTER: Movies directed by '{nolan.Name}':");
    var nolanMovies = await movService.GetMovieByDirectorAsync(nolan.Id);
    foreach (var m in nolanMovies)
    {
        Console.WriteLine($" - {m.Title}");
    }



    Console.WriteLine("\n[3]. Update a Movie...");
    allMovies = await movService.GetAllMovieAsync();
    if (allMovies.Any())
    {
        var movieToUpdate = allMovies.First(); // e.g. Inception
        var originalTitle = movieToUpdate.Title;

        movieToUpdate.Title = originalTitle + " (Director's Cut)";
        movieToUpdate.ReleaseYear = 2025;

        var updatedMovie = await movService.UpdateMovieAsync(movieToUpdate.Id, movieToUpdate);
        if (updatedMovie != null)
        {
            Console.WriteLine($"\n Updated Movie ID {updatedMovie.Id}:");
            Console.WriteLine($"   Title: {originalTitle} → {updatedMovie.Title}");
            Console.WriteLine($"   Year:  {updatedMovie.ReleaseYear}");
        }
    }


    Console.WriteLine("\n Delete MOVIE...");
    // ========================================================
    // DELETE MOVIE DEMO
    // ========================================================

    // 1. Select the last movie to delete
    allMovies = await movService.GetAllMovieAsync();
    var deleteThisMovie = allMovies.Last();
    Console.WriteLine($"Selected movie to delete: ID {deleteThisMovie.Id} - '{deleteThisMovie.Title}'");

    // 2. Delete it
    var isDeleted = await movService.DeleteMovieAsync(deleteThisMovie.Id);

    if (isDeleted)
    {
        Console.WriteLine($"Successfully deleted Movie ID {deleteThisMovie.Id} ('{deleteThisMovie.Title}')");
    }

    // 3. Verify it is really gone from SQLite
    var verifyDeleted = await movService.GetMovieByIdAsync(deleteThisMovie.Id);
    if (verifyDeleted == null)
    {
        Console.WriteLine("Check: Movie no longer exists in database!");
    }
}
