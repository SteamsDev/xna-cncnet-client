﻿namespace ClientCore.Statistics
{
    public abstract class GenericMatchParser : IMatchStatisticsParser
    {
        public MatchStatistics Statistics {get; set;}

        public GenericMatchParser(MatchStatistics ms)
        {
            Statistics = ms;
        }

        public abstract void ParseStatistics(string gamepath);
    }
}
