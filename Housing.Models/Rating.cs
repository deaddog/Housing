using System;

namespace Housing.Models
{
    public class Rating
    {
        public Rating(double score, double maxScore)
        {
            if (maxScore <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxScore), "Rating max score must be greater than zero.");

            if (score <= 0)
                throw new ArgumentOutOfRangeException(nameof(score), "Rating score must be greater than zero.");

            if (score > maxScore)
                throw new ArgumentOutOfRangeException(nameof(score), "Rating score cannot be greater than the max score.");

            Score = score;
            MaxScore = maxScore;
        }

        public double Score { get; }
        public double MaxScore { get; }

        public double GetRating(double maxScore)
        {
            return Score * (maxScore / MaxScore);
        }
    }
}
