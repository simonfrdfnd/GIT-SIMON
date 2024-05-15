namespace NexioMax3.Domain.Configuration.Model
{
  using System;
  using System.Linq;
  using Newtonsoft.Json;

  public class ManualPointConfig
  {
    [JsonProperty(nameof(HeightDefaultDelta), DefaultValueHandling = DefaultValueHandling.Ignore)]
    public double HeightDefaultDelta { get; set; }

    [JsonProperty(nameof(AngleDefaultDelta), DefaultValueHandling = DefaultValueHandling.Ignore)]
    public double AngleDefaultDelta { get; set; }

    [JsonProperty(nameof(MinHeightDefault), DefaultValueHandling = DefaultValueHandling.Ignore)]
    public double MinHeightDefault { get; set; } = 1;

    [JsonProperty(nameof(MinAngleDefault), DefaultValueHandling = DefaultValueHandling.Ignore)]
    public double MinAngleDefault { get; set; } = 0;

    [JsonProperty(nameof(Comments), DefaultValueHandling = DefaultValueHandling.Ignore)]
    public ManualPointComment[] Comments { get; set; } = new ManualPointComment[0];

    [JsonProperty(nameof(DisabledColumns), DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string[] DisabledColumns { get; set; } = new string[0];

    [JsonProperty(nameof(UseArrows), DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool UseArrows { get; set; } = true; // Autorise l'utilisation des touches fléchées pour bouger les positionneurs

    [JsonProperty(nameof(IsSourceByDetector))]
    public bool IsSourceByDetector { get; set; } = false;

    public string[] GetProjectComments(Guid projectId)
    {
      var mpc = this.Comments.FirstOrDefault(comment => comment.ProjectId == projectId);

      if (mpc == null)
      {
        mpc = new ManualPointComment()
        {
          ProjectId = projectId,
        };

        var c = this.Comments.ToList();
        c.Add(mpc);
        this.Comments = c.ToArray();
      }

      return mpc.Comments;
    }

    public void SaveProjectComments(Guid projectId, string[] comments)
    {
      var mpc = this.Comments.FirstOrDefault(comment => comment.ProjectId == projectId);
      if (mpc == null)
      {
        mpc = new ManualPointComment()
        {
          ProjectId = projectId,
        };

        var c = this.Comments.ToList();
        c.Add(mpc);
        this.Comments = c.ToArray();
      }

      mpc.Comments = comments;
    }

    public void AddComment(Guid projectId, string comment)
    {
      var comments = this.GetProjectComments(projectId).ToList();

      if (comments.Any(s => s == comment))
      {
        return;
      }

      comments.Add(comment);

      this.SaveProjectComments(projectId, comments.ToArray());
    }
  }
}