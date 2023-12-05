// See https://aka.ms/new-console-template for more information

using System.Diagnostics;

record Range(long Start, long End, long Offset);
record Seed(long Start, long End);

class test
{
    public void Run2_Intervals_Splitting(string file)
    {
        var lines = File.ReadAllLines(file);
        var seedsParts = lines[0].Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[1]
            .Split(' ')
            .Select(long.Parse)
            .ToArray();

        var seeds = seedsParts
            .Chunk(2)
            .Select(x => new Seed(x[0], x[0] + x[1] - 1))
            .ToArray();

        var maps = new List<List<Range>>();
        bool category = false;

        foreach (string line in lines[1..])
        {
            if (line == "")
            {
                category = true;
            }
            else if (category)
            {
                category = false;
                maps.Add(new List<Range>());
            }
            else
            {
                var parts = line.Split(' ');
                long destStart = long.Parse(parts[0]);
                long srcStart = long.Parse(parts[1]);
                long rangeLen = long.Parse(parts[2]);
                var range = new Range(srcStart, srcStart + rangeLen - 1, destStart - srcStart);
                maps.Last().Add(range);
            }
        }

        var seedsIn = seeds.ToList();
        var seedsOut = new List<Seed>();

        foreach (List<Range> ranges in maps)
        {
            var queue = new Queue<Seed>();
            foreach (Seed seed in seedsIn)
                queue.Enqueue(seed);

            while (queue.Count > 0)
            {
                var seed = queue.Dequeue();
                if (seed.End < seed.Start)
                    continue;

                var r1 = ranges.FirstOrDefault(range => seed.Start >= range.Start && seed.Start <= range.End);
                var r2 = ranges.FirstOrDefault(range => seed.End >= range.Start && seed.End <= range.End);

                if (r1 is null && r2 is null)
                {
                    seedsOut.Add(seed);
                }
                else if (r1 is not null && r2 is not null)
                {
                    if (r1 == r2)
                    {
                        seedsOut.Add(new Seed(seed.Start + r1.Offset, seed.End + r1.Offset));
                    }
                    else
                    {
                        seedsOut.Add(new Seed(seed.Start + r1.Offset, r1.End + r1.Offset));
                        queue.Enqueue(new Seed(r1.End + 1, r2.Start - 1));
                        seedsOut.Add(new Seed(r2.Start + r2.Offset, seed.End + r2.Offset));
                    }
                }
                else if (r1 is not null)
                {
                    seedsOut.Add(new Seed(seed.Start + r1.Offset, r1.End + r1.Offset));
                    queue.Enqueue(new Seed(r1.End + 1, seed.End));
                }
                else if (r2 is not null)
                {
                    queue.Enqueue(new Seed(seed.Start, r2.Start - 1));
                    seedsOut.Add(new Seed(r2.Start + r2.Offset, seed.End + r2.Offset));
                }
            }

            seedsIn = seedsOut.Where(seed => seed.End >= seed.Start).ToList();
            seedsOut.Clear();
        }

        long min = seedsIn.Min(seed => seed.Start);

        Debug.WriteLine(min);
    }

}

class Program
{
    static void Main(string[] args)
    {
        string filePath = @"C:\testutvikling\Diagnostics\Diagnostics\day5.txt";
        ;
       // Replace with the actual path of your input file
        var test = new test(); // Replace with the actual class name

        test.Run2_Intervals_Splitting(filePath);
    }
}
