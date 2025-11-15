using BussinessObject;

namespace Services
{
    public class BlogService : IBlogService
    {
        private readonly List<BlogPost> _blogPosts;

        public BlogService()
        {
            _blogPosts = InitializeMockBlogPosts();
        }

        public Task<IEnumerable<BlogPost>> GetAllPostsAsync()
        {
            return Task.FromResult(_blogPosts.Where(p => p.IsPublished == true)
                .OrderByDescending(p => p.CreatedAt ?? DateTime.MinValue)
                .AsEnumerable());
        }

        public Task<BlogPost?> GetPostByIdAsync(int id)
        {
            return Task.FromResult(_blogPosts.FirstOrDefault(p => p.Id == id && p.IsPublished == true));
        }

        public Task<IEnumerable<BlogPost>> GetRecentPostsAsync(int count = 3)
        {
            return Task.FromResult(_blogPosts.Where(p => p.IsPublished == true)
                .OrderByDescending(p => p.CreatedAt ?? DateTime.MinValue)
                .Take(count)
                .AsEnumerable());
        }

        private List<BlogPost> InitializeMockBlogPosts()
        {
            var posts = new List<BlogPost>
            {
                new BlogPost
                {
                    Id = 1,
                    Title = "How to Choose the Perfect Tennis Racket",
                    Content = @"<p>Choosing the right tennis racket is crucial for improving your game. Here are the key factors to consider:</p>
                        <h3>1. Racket Weight</h3>
                        <p>Lightweight rackets (8-9.5 oz) are easier to swing and great for beginners. Heavier rackets (10-12 oz) provide more power and stability for advanced players.</p>
                        <h3>2. Head Size</h3>
                        <p>Midsize (85-95 sq in) offers more control, while oversized (100-110 sq in) provides a larger sweet spot and more power. Mid-plus (95-100 sq in) is a popular middle ground.</p>
                        <h3>3. Grip Size</h3>
                        <p>Measure from the middle crease of your palm to the tip of your ring finger. The right grip size prevents injuries and improves control.</p>
                        <h3>4. String Pattern</h3>
                        <p>Open string patterns (16x18) offer more spin, while dense patterns (18x20) provide better control and durability.</p>
                        <p>Remember to test different rackets to find what feels comfortable for your playing style!</p>",
                    Excerpt = "Learn the essential factors to consider when selecting your ideal tennis racket for maximum performance on the court.",
                    Author = "John Tennis Pro",
                    Category = "Tennis Tips",
                    ImageUrl = "/images/blog1.jpg",
                    CreatedAt = DateTime.Now.AddDays(-5),
                    IsPublished = true,
                    Views = 1245
                },
                new BlogPost
                {
                    Id = 2,
                    Title = "Top 10 Tennis Training Exercises for Better Performance",
                    Content = @"<p>Improving your tennis game requires consistent training both on and off the court. Here are the top 10 exercises:</p>
                        <h3>1. Lateral Lunges</h3>
                        <p>Improves side-to-side movement and leg strength essential for court coverage.</p>
                        <h3>2. Medicine Ball Throws</h3>
                        <p>Builds core strength and power for serves and groundstrokes.</p>
                        <h3>3. Agility Ladder Drills</h3>
                        <p>Enhances footwork speed and coordination.</p>
                        <h3>4. Resistance Band Rotations</h3>
                        <p>Strengthens shoulder muscles and prevents injuries.</p>
                        <h3>5. Box Jumps</h3>
                        <p>Develops explosive power for quick movements on court.</p>
                        <h3>6. Plank Variations</h3>
                        <p>Builds core stability for better balance and power transfer.</p>
                        <h3>7. Russian Twists</h3>
                        <p>Improves rotational strength for powerful shots.</p>
                        <h3>8. Burpees</h3>
                        <p>Builds overall endurance and cardiovascular fitness.</p>
                        <h3>9. Single-Leg Deadlifts</h3>
                        <p>Improves balance and stability during shots.</p>
                        <h3>10. Interval Sprints</h3>
                        <p>Mimics match conditions and builds match-specific endurance.</p>
                        <p>Incorporate these exercises into your training routine 3-4 times per week for best results!</p>",
                    Excerpt = "Discover the most effective training exercises to improve your tennis game and build strength and endurance.",
                    Author = "Sarah Coach",
                    Category = "Training",
                    ImageUrl = "/images/blog2.jpg",
                    CreatedAt = DateTime.Now.AddDays(-10),
                    IsPublished = true,
                    Views = 2156
                },
                new BlogPost
                {
                    Id = 3,
                    Title = "Essential Tennis Equipment for Beginners",
                    Content = @"<p>Starting your tennis journey? Here's everything you need to get started:</p>
                        <h3>1. Tennis Racket</h3>
                        <p>Choose a lightweight, oversized racket with a comfortable grip. Pre-strung rackets are perfect for beginners.</p>
                        <h3>2. Tennis Shoes</h3>
                        <p>Court-specific shoes with good traction and support are essential to prevent injuries and improve movement.</p>
                        <h3>3. Tennis Balls</h3>
                        <p>Start with pressureless balls for practice, or regular pressurized balls for match play.</p>
                        <h3>4. Athletic Clothing</h3>
                        <p>Moisture-wicking shirts and shorts keep you comfortable during long sessions.</p>
                        <h3>5. Water Bottle</h3>
                        <p>Stay hydrated! Tennis is physically demanding, especially in hot weather.</p>
                        <h3>6. Tennis Bag</h3>
                        <p>Keep all your equipment organized with a proper tennis bag.</p>
                        <h3>7. Overgrips</h3>
                        <p>Replace worn grips to maintain racket control and comfort.</p>
                        <h3>8. Wristbands</h3>
                        <p>Keep sweat off your hands and racket grip.</p>
                        <p>Start with these essentials and add more specialized equipment as you progress!</p>",
                    Excerpt = "Everything you need to get started in tennis, from rackets to shoes and accessories for new players.",
                    Author = "Mike Equipment Expert",
                    Category = "Equipment",
                    ImageUrl = "/images/blog3.jpg",
                    CreatedAt = DateTime.Now.AddDays(-15),
                    IsPublished = true,
                    Views = 1890
                },
                new BlogPost
                {
                    Id = 4,
                    Title = "Tennis String Tension: Finding Your Perfect Setup",
                    Content = @"<p>String tension significantly affects your racket's performance. Here's how to find your ideal tension:</p>
                        <h3>Low Tension (40-50 lbs)</h3>
                        <p>Provides more power and a larger sweet spot. Great for players who need extra power and spin.</p>
                        <h3>Medium Tension (50-55 lbs)</h3>
                        <p>Balanced power and control. This is the most popular tension range for recreational players.</p>
                        <h3>High Tension (55-65 lbs)</h3>
                        <p>Offers more control and precision but less power. Preferred by advanced players with strong swings.</p>
                        <h3>Factors to Consider</h3>
                        <ul>
                            <li>Your playing style (power vs control)</li>
                            <li>String type (polyester, synthetic gut, natural gut)</li>
                            <li>Racket stiffness</li>
                            <li>Weather conditions (tension changes with temperature)</li>
                        </ul>
                        <p>Experiment with different tensions to find what works best for your game!</p>",
                    Excerpt = "Learn how string tension affects your racket performance and find the perfect setup for your playing style.",
                    Author = "Tennis Pro Staff",
                    Category = "Equipment",
                    ImageUrl = "/images/blog4.jpg",
                    CreatedAt = DateTime.Now.AddDays(-20),
                    IsPublished = true,
                    Views = 987
                },
                new BlogPost
                {
                    Id = 5,
                    Title = "Tennis Footwork: The Foundation of Great Play",
                    Content = @"<p>Proper footwork is the foundation of excellent tennis. Here's how to improve:</p>
                        <h3>1. Split Step</h3>
                        <p>Jump slightly as your opponent hits the ball to prepare for quick directional changes.</p>
                        <h3>2. Recovery Position</h3>
                        <p>Always return to the center of the court after each shot to maintain court coverage.</p>
                        <h3>3. Crossover Steps</h3>
                        <p>Use crossover steps for longer distances, keeping your body balanced and ready.</p>
                        <h3>4. Small Adjustment Steps</h3>
                        <p>Use small steps to fine-tune your position before hitting the ball.</p>
                        <h3>5. Lateral Movement</h3>
                        <p>Practice side shuffles to improve your ability to reach wide shots.</p>
                        <h3>Common Footwork Mistakes</h3>
                        <ul>
                            <li>Being flat-footed</li>
                            <li>Not recovering to center</li>
                            <li>Taking too large steps</li>
                            <li>Not using split step</li>
                        </ul>
                        <p>Practice these footwork patterns regularly to see significant improvement in your game!</p>",
                    Excerpt = "Master the essential footwork patterns that form the foundation of excellent tennis performance.",
                    Author = "Coach Lisa",
                    Category = "Training",
                    ImageUrl = "/images/blog5.jpg",
                    CreatedAt = DateTime.Now.AddDays(-25),
                    IsPublished = true,
                    Views = 1432
                },
                new BlogPost
                {
                    Id = 6,
                    Title = "Tennis Nutrition: Fueling Your Performance",
                    Content = @"<p>Proper nutrition is essential for peak tennis performance. Here's what to eat:</p>
                        <h3>Before Playing</h3>
                        <p>Eat a meal rich in carbohydrates 2-3 hours before playing. Good options include pasta, rice, or oatmeal. Avoid heavy, fatty foods.</p>
                        <h3>During Matches</h3>
                        <p>Stay hydrated with water and electrolyte drinks. Eat light snacks like bananas, energy bars, or sports drinks during breaks.</p>
                        <h3>After Playing</h3>
                        <p>Within 30 minutes after playing, consume protein and carbohydrates to aid recovery. Chocolate milk, protein shakes, or a balanced meal work well.</p>
                        <h3>Daily Nutrition Tips</h3>
                        <ul>
                            <li>Stay hydrated throughout the day</li>
                            <li>Eat plenty of fruits and vegetables</li>
                            <li>Include lean proteins in every meal</li>
                            <li>Choose whole grains over refined carbs</li>
                            <li>Avoid excessive sugar and processed foods</li>
                        </ul>
                        <p>Remember: nutrition is just as important as training for optimal performance!</p>",
                    Excerpt = "Learn how proper nutrition can enhance your tennis performance and aid in recovery.",
                    Author = "Nutrition Expert",
                    Category = "Health",
                    ImageUrl = "/images/blog6.jpg",
                    CreatedAt = DateTime.Now.AddDays(-30),
                    IsPublished = true,
                    Views = 1123
                },
                new BlogPost
                {
                    Id = 7,
                    Title = "Tennis Serve: Mastering the Power Serve",
                    Content = @"<p>The serve is one of the most important shots in tennis. Here's how to master it:</p>
                        <h3>1. Grip</h3>
                        <p>Use a continental grip (holding the racket like a hammer) for maximum power and spin.</p>
                        <h3>2. Stance</h3>
                        <p>Stand sideways to the net with your front foot pointing toward the net post. Your back foot should be parallel to the baseline.</p>
                        <h3>3. Toss</h3>
                        <p>Toss the ball slightly in front and to the right (for right-handed players). The ball should reach its peak at full arm extension.</p>
                        <h3>4. Motion</h3>
                        <p>Use a smooth, fluid motion. Pull your racket back, bend your knees, then explode upward into the ball.</p>
                        <h3>5. Contact Point</h3>
                        <p>Contact the ball at full extension, slightly in front of your body for maximum power.</p>
                        <h3>6. Follow-Through</h3>
                        <p>Follow through across your body and finish with your racket on the opposite side.</p>
                        <p>Practice these fundamentals consistently to develop a powerful, reliable serve!</p>",
                    Excerpt = "Learn the fundamentals of a powerful tennis serve and how to improve your serving technique.",
                    Author = "Pro Coach David",
                    Category = "Tennis Tips",
                    ImageUrl = "/images/blog7.jpg",
                    CreatedAt = DateTime.Now.AddDays(-35),
                    IsPublished = true,
                    Views = 2056
                },
                new BlogPost
                {
                    Id = 8,
                    Title = "Choosing the Right Tennis Shoes: A Complete Guide",
                    Content = @"<p>Your tennis shoes are crucial for performance and injury prevention. Here's what to look for:</p>
                        <h3>1. Court-Specific Design</h3>
                        <p>Tennis shoes are designed for lateral movement and quick stops. Running shoes won't provide the support you need.</p>
                        <h3>2. Outsole Type</h3>
                        <p>Hard court shoes have durable, non-marking soles. Clay court shoes have herringbone patterns. Grass court shoes have pimples for grip.</p>
                        <h3>3. Cushioning</h3>
                        <p>Look for adequate cushioning in the heel and forefoot to absorb impact and prevent injuries.</p>
                        <h3>4. Stability</h3>
                        <p>Good tennis shoes provide lateral support to prevent ankle rolls during quick direction changes.</p>
                        <h3>5. Fit</h3>
                        <p>Your shoes should fit snugly but not tight. Leave about a thumb's width of space at the toe.</p>
                        <h3>6. Durability</h3>
                        <p>Look for reinforced toe caps and durable materials that can withstand court abrasion.</p>
                        <p>Replace your tennis shoes every 45-60 hours of play to maintain proper support and cushioning!</p>",
                    Excerpt = "Find the perfect tennis shoes for your playing style and court surface with this comprehensive guide.",
                    Author = "Equipment Specialist",
                    Category = "Equipment",
                    ImageUrl = "/images/blog8.jpg",
                    CreatedAt = DateTime.Now.AddDays(-40),
                    IsPublished = true,
                    Views = 1678
                },
                new BlogPost
                {
                    Id = 9,
                    Title = "Tennis Mental Game: Developing Mental Toughness",
                    Content = @"<p>Mental toughness is just as important as physical skills in tennis. Here's how to develop it:</p>
                        <h3>1. Focus on the Present</h3>
                        <p>Don't dwell on past mistakes or worry about future points. Focus only on the current point.</p>
                        <h3>2. Develop a Pre-Serve Routine</h3>
                        <p>Create a consistent routine before serving to calm your nerves and improve focus.</p>
                        <h3>3. Manage Emotions</h3>
                        <p>Stay calm after errors. Accept that mistakes are part of the game and move on quickly.</p>
                        <h3>4. Positive Self-Talk</h3>
                        <p>Replace negative thoughts with positive affirmations. Encourage yourself during matches.</p>
                        <h3>5. Visualization</h3>
                        <p>Visualize successful shots and match situations during practice to build confidence.</p>
                        <h3>6. Breathing Techniques</h3>
                        <p>Use deep breathing to stay calm under pressure, especially during important points.</p>
                        <h3>7. Set Process Goals</h3>
                        <p>Focus on process goals (like good footwork) rather than outcome goals (winning) to reduce pressure.</p>
                        <p>Mental toughness takes practice, but it's a skill that can significantly improve your game!</p>",
                    Excerpt = "Learn mental strategies to improve your focus, confidence, and performance on the tennis court.",
                    Author = "Sports Psychologist",
                    Category = "Mental Game",
                    ImageUrl = "/images/blog9.jpg",
                    CreatedAt = DateTime.Now.AddDays(-45),
                    IsPublished = true,
                    Views = 1345
                },
                new BlogPost
                {
                    Id = 10,
                    Title = "Tennis Injury Prevention: Stay Healthy on the Court",
                    Content = @"<p>Preventing injuries is crucial for long-term tennis enjoyment. Here's how to stay healthy:</p>
                        <h3>1. Warm-Up Properly</h3>
                        <p>Always warm up for 10-15 minutes before playing. Include light cardio, dynamic stretches, and practice swings.</p>
                        <h3>2. Cool Down</h3>
                        <p>After playing, do static stretches to improve flexibility and prevent muscle tightness.</p>
                        <h3>3. Strength Training</h3>
                        <p>Build strength in your core, shoulders, and legs to support your tennis movements and prevent injuries.</p>
                        <h3>4. Proper Technique</h3>
                        <p>Learn proper form to avoid putting unnecessary stress on your joints and muscles.</p>
                        <h3>5. Rest and Recovery</h3>
                        <p>Allow adequate rest between matches and training sessions. Overtraining leads to injuries.</p>
                        <h3>6. Hydration</h3>
                        <p>Stay hydrated before, during, and after playing to prevent cramps and fatigue.</p>
                        <h3>7. Listen to Your Body</h3>
                        <p>If you feel pain, stop playing and rest. Don't push through injuries.</p>
                        <h3>Common Tennis Injuries</h3>
                        <ul>
                            <li>Tennis elbow (lateral epicondylitis)</li>
                            <li>Shoulder impingement</li>
                            <li>Ankle sprains</li>
                            <li>Knee injuries</li>
                            <li>Lower back pain</li>
                        </ul>
                        <p>Prevention is always better than treatment. Take care of your body to enjoy tennis for years to come!</p>",
                    Excerpt = "Learn how to prevent common tennis injuries and stay healthy on the court with proper preparation and technique.",
                    Author = "Sports Medicine Expert",
                    Category = "Health",
                    ImageUrl = "/images/blog10.jpg",
                    CreatedAt = DateTime.Now.AddDays(-50),
                    IsPublished = true,
                    Views = 2234
                }
            };

            return posts;
        }
    }
}
