using System;

namespace Housing.Models
{
    public class CategoryRating
    {
        public CategoryRating(string user, string category, Rating rating)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            Category = category ?? throw new ArgumentNullException(nameof(category));
            Rating = rating ?? throw new ArgumentNullException(nameof(rating));
        }
        
        public string User { get; }
        public string Category { get; }
        public Rating Rating { get; }
    }
}
