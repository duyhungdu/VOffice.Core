using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOffice.Model
{
    public class MenuRole
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Title { get; set; }
        public bool Root { get; set; }
        public string Code { get; set; }
        public bool Leaf { get; set; }
        public string Href { get; set; }
        public string Target { get; set; }
        public string Icon { get; set; }
        public int Order { get; set; }
        public bool OnMainMenu { get; set; }
        public bool OnTopMenu { get; set; }
        public bool OnRightMenu { get; set; }
        public bool Active { get; set; }

        public List<MenuRole> Categories = new List<MenuRole>();
        public MenuRole Parent { get; set; }
        public bool ShouldSerializeChildren()
        {
            return (Categories.Count > 0);
        }

        public static IEnumerable<MenuRole> RawCollectionToTree(List<MenuRole> collection)
        {
            var treeDictionary = new Dictionary<string, MenuRole>();

            collection.ForEach(x => treeDictionary.Add(x.Id,
                               new MenuRole
                               {
                                   Id = x.Id,
                                   ParentId = x.ParentId,
                                   Title = x.Title,
                                   Active = x.Active,
                                   Code = x.Code,
                                   Href = x.Href,
                                   Icon = x.Icon,
                                   Leaf = x.Leaf,
                                   OnMainMenu = x.OnMainMenu,
                                   OnRightMenu = x.OnRightMenu,
                                   OnTopMenu = x.OnTopMenu,
                                   Order = x.Order,
                                   Root = x.Root,
                                   Target = x.Target
                               }));

            foreach (var item in treeDictionary.Values)
            {
                if (item.ParentId != "#")
                {
                    MenuRole proposedParent;

                    if (treeDictionary.TryGetValue(item.ParentId, out proposedParent))
                    {
                        item.Parent = proposedParent;

                        proposedParent.Categories.Add(item);
                    }
                }

            }
            return treeDictionary.Values.Where(x => x.Parent == null);

        }
    }
}
