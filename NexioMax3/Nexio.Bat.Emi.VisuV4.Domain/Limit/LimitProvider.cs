namespace Nexio.Bat.Emi.VisuV4.Domain.Limit
{
  using System.Collections.Generic;
  using System.Linq;
  using Nexio.Bat.Common.Domain.ATDB.Model;
  using Nexio.Bat.Common.Domain.Grammar.Model;
  using Nexio.Bat.Common.Domain.Grammar.Service;
  using Nexio.Bat.Emi.VisuV4.Domain.Model;
  using Nexio.Bat.Emi.VisuV4.Domain.Service;

  public class LimitProvider
  {
    private static LimitProvider instance;

    private LimitRepository limitRepository;

    private LimitProvider()
    {
      this.limitRepository = new LimitRepository();
    }

    public static LimitProvider Instance => instance ?? (instance = new LimitProvider());

    public List<Node> GetLimitsGrammar(ObjectType objectType)
    {
      var limits = Provider.Instance.GetLimits();
      var limitGrammarNodes = GrammarProvider.Instance.GetNodeByIdTypeObject(45, 50).ToList();

      foreach (var node in limitGrammarNodes)
      {
        this.RemoveChildrenNotContainingLimits(node, limits);
      }

      limitGrammarNodes = limitGrammarNodes.Where(node => node.ChildList.Count > 0).ToList();

      return limitGrammarNodes;
    }

    public List<int> GetLimitFolderIds()
    {
      return this.limitRepository.GetLimitFolderIds();
    }

    private void RemoveChildrenNotContainingLimits(Node node, List<Limit> limits)
    {
      if (node.ChildList == null || node.ChildList.Count == 0)
      {
        return;
      }

      int i = 0;

      while (i < node.ChildList.Count)
      {
        var childNode = node.ChildList[i];
        if (limits.Any(limit => limit.Id == childNode.ID))
        {
          i++;
          continue;
        }

        if (childNode.ChildList == null || childNode.ChildList.Count == 0)
        {
          node.ChildList.Remove(childNode);
          continue;
        }

        this.RemoveChildrenNotContainingLimits(childNode, limits);

        if (childNode.ChildList.Count == 0)
        {
          node.ChildList.Remove(childNode);
          continue;
        }

        i++;
      }
    }
  }
}