using UnityEngine;

namespace Resource {
    public class ResourceManager : SingleTon<ResourceManager> {
        private readonly Sprite[] _ranks;
        private static readonly Color[] RankColor = {
            new(0.9764706f, 0.8470589f, 0.5058824f),
            new(0.9176471f, 0.6235294f, 0.9921569f),
            new(0.9176471f, 0.6235294f, 0.9921569f),
            new(0.6117647f, 0.8901961f, 1f),
            new(0.6117647f, 0.8901961f, 1f),
            new(0.7490196f, 0.9764706f, 0.5529412f),
            new(0.7490196f, 0.9764706f, 0.5529412f),
            new(0.9686275f, 0.4392157f, 0.4392157f),
            new(0.9686275f, 0.4392157f, 0.4392157f),
            new(0.7882354f, 0.7882354f, 0.7882354f)
        };
        
        public ResourceManager() {
            _ranks = new[] {
                Resources.Load<Sprite>("Rank/S"),
                Resources.Load<Sprite>("Rank/Aplus"),
                Resources.Load<Sprite>("Rank/A"),
                Resources.Load<Sprite>("Rank/Bplus"),
                Resources.Load<Sprite>("Rank/B"),
                Resources.Load<Sprite>("Rank/Cplus"),
                Resources.Load<Sprite>("Rank/C"),
                Resources.Load<Sprite>("Rank/Dplus"),
                Resources.Load<Sprite>("Rank/D"),
                Resources.Load<Sprite>("Rank/F")
            };
        }

        public Sprite GetRank(int rank) => _ranks[rank];

        public Color GetRankColor(int rank) => RankColor[rank];
    }
}