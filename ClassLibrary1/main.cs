using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ascon.Pilot.SDK;
using Ascon.Pilot.SDK.Menu;
using System.Windows;
using System.IO;
using System.ComponentModel.Composition;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using Ascon.Pilot.SDK.Automation;
using Ascon.Pilot.SDK.Data;
using update_status;

namespace update_status
{
    [Export(typeof(IMenu<ObjectsViewContext>))]
    public class MainModule : IMenu<ObjectsViewContext>
    {
        private readonly IObjectsRepository _repository;
        private readonly IObjectModifier _modifier;
        private readonly ISearchService _searchService;
        private const string CREATE_COMMAND = "CreateCommand";
        private readonly List<Guid> _selectionIds = new List<Guid>();
        private readonly string configSettings;


        [ImportingConstructor]
        public MainModule(IObjectsRepository repository, IObjectModifier modifier, ISearchService searchService)
        {
            _repository = repository;
            _modifier = modifier;
            _searchService = searchService;
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dir = $@"{appData}\ASCON\Pilot-BIM\update_status\";
            configSettings = configFiles(dir, "settings.json");


        }

        public void Build(IMenuBuilder builder, ObjectsViewContext context)
        {
            var createItemBuilder = builder.AddItem(CREATE_COMMAND, 0).WithHeader("update status").WithSubmenu();

            var selectionList = context.SelectedObjects.ToList();
            //_firstParentId = selectionList.First().ParentId;

        }

        public void OnMenuItemClick(string name, ObjectsViewContext context)
        {



            if (name == CREATE_COMMAND)
            {

                //Читаем данные из json и преобразовываем их в объекта класса, для последующего добавления в Pilot

                string jsonSettings = File.ReadAllText(configSettings);
                SettingsObject settingsData = JsonConvert.DeserializeObject<SettingsObject>(jsonSettings);
                //Выбираем объект, который будет родительский. Этот тот по-которому мы щелкнули
                var selectObjects = context.SelectedObjects.ToList();

                foreach (var selectObject in selectObjects) {
                    _modifier.Edit(selectObject)
                        .SetAttribute(settingsData.name_attr_state, Guid.Parse(settingsData.id_state));
                    _modifier.Apply();
                        }

                
            }


        }

        private string configFiles(string dir, string file)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (!File.Exists(dir + file))
                File.Create(dir + file).Close();
            return dir + file;

        }

        /*private bool searchJson(List<IDataObject> objJson)
        {
            return objJson.Contains(item. ;
        }*/

    }
}

