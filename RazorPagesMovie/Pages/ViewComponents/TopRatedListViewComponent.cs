using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesMovie.ViewComponents
{
    public class TopRatedListViewComponent : ViewComponent
    {
        private readonly RazorPagesMovieContext db;

        public TopRatedListViewComponent(RazorPagesMovieContext context)
        {
            db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int itemCount)
        {
            var items = await GetItemsAsync(itemCount);
            return View(items);
        }

        private Task<List<Movie>> GetItemsAsync(int itemCount)
        {
            return db.Movie.OrderByDescending(m => m.Rating)
                           .ThenBy(m => m.Title)
                           .Take(itemCount)
                           .ToListAsync();
        }
    }
}
