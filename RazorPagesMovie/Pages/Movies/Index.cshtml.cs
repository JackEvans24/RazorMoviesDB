using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Controls;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Pages.Movies
{
    public class IndexModel : PageModel
    {
        private readonly RazorPagesMovie.Models.RazorPagesMovieContext _context;

        public IndexModel(RazorPagesMovie.Models.RazorPagesMovieContext context)
        {
            _context = context;
        }

        public PaginatedList<Movie> Movie { get;set; }
        public SelectList Genres { get; set; }
        public string MovieGenre { get; set; }
        
        public string CurrentSort { get; set; }
        public string TitleSort { get; set; }
        public string ReleaseSort { get; set; }
        public string GenreSort { get; set; }
        public string PriceSort { get; set; }
        public string RatingSort { get; set; }

        public string CurrentFilter { get; set; }
        public int PageSize { get; set; }

        public async Task OnGetAsync(string movieGenre, string currentFilter, string searchString, string sortOrder, int? pageIndex, int? pageSize)
        {
            if (searchString != null)
            {
                pageIndex = 1;
                CurrentFilter = searchString;
            }
            else
                searchString = CurrentFilter = currentFilter;

            if (pageSize != null && pageSize > 0)
                PageSize = pageSize ?? 25;
            else
                pageSize = PageSize = 25;

            var genreQuery = _context.Movie.SelectMany(m => m.Genre.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                                                   .OrderBy(g => g);
            var movies = from m in _context.Movie
                         select m;

            if (!string.IsNullOrWhiteSpace(searchString))
                movies = movies.Where(m => m.Title.Contains(searchString));

            if (!string.IsNullOrWhiteSpace(movieGenre))
                movies = movies.Where(m =>  m.Genre.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                   .Any(g => g == movieGenre));

            movies = SortMovies(movies, sortOrder);

            Genres = new SelectList(await genreQuery.Distinct().ToListAsync());
            Movie = await PaginatedList<Movie>.CreateAsync(movies.AsNoTracking(), pageIndex ?? 1, pageSize ?? 25);
        }

        public async Task OnGetClearAsync(int? pageSize)
        {
            CurrentFilter = null;
            MovieGenre = null;

            var genreQuery = _context.Movie.SelectMany(m => m.Genre.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                                                   .OrderBy(g => g);
            var movies = from m in _context.Movie
                         select m;

            movies = SortMovies(movies, "");

            Genres = new SelectList(await genreQuery.Distinct().ToListAsync());
            Movie = await PaginatedList<Movie>.CreateAsync(movies.AsNoTracking(), 1, pageSize > 0 ? pageSize ?? 25 : 25);
        }

        private IQueryable<Movie> SortMovies(IQueryable<Movie> movies, string sortOrder)
        {
            CurrentSort = sortOrder;

            TitleSort = string.IsNullOrWhiteSpace(sortOrder) ? "title_desc" : "";
            ReleaseSort = sortOrder == "release" ? "release_desc" : "release";
            GenreSort = sortOrder == "genre" ? "genre_desc" : "genre";
            PriceSort = sortOrder == "price" ? "price_desc" : "price";
            RatingSort = sortOrder == "rating" ? "rating_desc" : "rating";

            switch (sortOrder)
            {
                case "title_desc":
                    return movies.OrderByDescending(m => m.Title);
                case "release":
                    return movies.OrderBy(m => m.ReleaseDate).ThenBy(m => m.Title);
                case "release_desc":
                    return movies.OrderByDescending(m => m.ReleaseDate).ThenBy(m => m.Title);
                case "genre":
                    return movies.OrderBy(m => m.Genre).ThenBy(m => m.Title);
                case "genre_desc":
                    return movies.OrderByDescending(m => m.Genre).ThenBy(m => m.Title);
                case "price":
                    return movies.OrderBy(m => m.Price).ThenBy(m => m.Title);
                case "price_desc":
                    return movies.OrderByDescending(m => m.Price).ThenBy(m => m.Title);
                case "rating":
                    return movies.OrderByDescending(m => m.Rating).ThenBy(m => m.Title);
                case "rating_desc":
                    return movies.OrderBy(m => m.Rating).ThenBy(m => m.Title);
                default:
                    return movies.OrderBy(m => m.Title);
            }
        }
    }
}
