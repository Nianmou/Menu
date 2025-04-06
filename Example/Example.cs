// 引入CounterStrikeSharp框架的相关命名空间  
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
// 引入菜单系统的相关命名空间  
using Menu;
using Menu.Enums;

namespace example;

public class Example : BasePlugin
{
    public override string ModuleName => "example";
    public override string ModuleVersion => "1.0.0";

    // 定义一个菜单对象，使用自动属性初始化  
    public Menu.Menu Menu { get; } = new();

    // 定义一个私有字典，用于存储玩家控制器和对应菜单的映射关系  
    private readonly Dictionary<CCSPlayerController, MenuBase> _dynamicMenu = new();

    // 为全局菜单的绘制事件添加一个委托（事件处理器） 
    public Example()
    {
        // 为全局菜单的绘制事件添加一个委托（事件处理器）  
        global::Menu.Menu.OnDrawMenu += (_, menuEvent) =>
        {
            // 获取触发此事件的控制器（可能是玩家控制器）  
            var controller = menuEvent.Controller;

            // 尝试从_dynamicMenu字典中获取与当前控制器相关联的动态菜单  
            if (!_dynamicMenu.TryGetValue(controller, out var dynamicMenu))
                return; // 如果找不到对应的动态菜单，则直接返回  

            // 检查当前触发的菜单事件是否与我们存储的动态菜单相匹配  
            if (menuEvent.Menu != dynamicMenu)
                return; // 如果不匹配，则直接返回  

            // 假设动态菜单的第一个项目包含一个DynamicValue类型的头部（Head）  
            // 这里使用了null条件运算符（!）来避免空引用异常  
            var dynamicValue = (DynamicValue)dynamicMenu.Items[0].Head!;

            // 设置dynamicValue的位置为控制器所对应玩家的当前绝对位置  
            dynamicValue.Position = controller.PlayerPawn.Value!.AbsOrigin!;
        };
    }

    public override void Load(bool isReload)
    {
        // 添加一个名为"css_test"的命令，该命令不带额外参数，执行一个Lambda表达式  
        AddCommand("css_test", "", (controller, _) =>
        {
            // 检查控制器是否为空或无效，如果是，则直接返回  
            if (controller == null || !controller.IsValid)
                return;

            // 定义菜单光标，包含两个值，每个值都带有前缀和后缀的HTML颜色标签  
            var cursor = new MenuValue[2]
            {
                new("►") { Prefix = "<font color=\"#3399FF\">", Suffix = "<font color=\"#FFFFFF\">" },
                new("◄") { Prefix = "<font color=\"#3399FF\">", Suffix = "<font color=\"#FFFFFF\">" },
            };

            // 定义菜单选择器，同样包含两个值，带有HTML颜色标签  
            var selector = new MenuValue[2]
            {
                new("[ ") { Prefix = "<font color=\"#0033FF\">", Suffix = "<font color=\"#FFFFFF\">" },
                new(" ]") { Prefix = "<font color=\"#0033FF\">", Suffix = "<font color=\"#FFFFFF\">" },
            };

            // 创建主菜单对象，设置菜单标题，并指定光标和选择器  
            var mainMenu = new MenuBase(new MenuValue("Main Menu") { Prefix = "<font class=\"fontSize-L\">", Suffix = "<font class=\"fontSize-sm\">" })
            {
                Cursor = cursor,
                Selector = selector
            };

            // 定义一组选项菜单值，每个值可以带有不同的前缀和后缀HTML颜色标签  
            var options = new List<MenuValue>
            {
                new("option1") { Prefix = "<font color=\"#9900FF\">", Suffix = "<font color=\"#FFFFFF\">" },
                new("option2"),
                new("option3"),
                new("option4") { Prefix = "<font color=\"#33AA33\">", Suffix = "<font color=\"#FFFFFF\">" },
                new("option5")
            };

            // 定义一组选择菜单值，同样带有颜色标签  
            var choices = new List<MenuValue>
            {
                new("choice1") { Prefix = "<font color=\"#AA1133\">", Suffix = "<font color=\"#FFFFFF\">" },
                new("choice2"),
                new("choice3"),
                new("choice4") { Prefix = "<font color=\"#BB9933\">", Suffix = "<font color=\"#FFFFFF\">" },
                new("choice5")
            };

            // 获取所有玩家，并将每个玩家转换为MenuValue对象，添加到players列表中  
            // 之后又添加了两个额外的玩家菜单项  
            var players = Utilities.GetPlayers().Select(player => (MenuValue)new PlayerValue(player.PlayerName, player.UserId)).ToList();
            players.Add(new PlayerValue("p1", 1) { Prefix = "<font color=\"#AA1133\">", Suffix = "<font color=\"#FFFFFF\">" });
            players.Add(new PlayerValue("p2", 2));

            // 创建多个菜单项，包括选项菜单、选择轮盘、玩家按钮、输入框和间隔  
            var itemOptions = new MenuItem(MenuItemType.ChoiceBool, options);
            var itemPinwheel = new MenuItem(MenuItemType.Choice, new MenuValue("h: "), choices, new MenuValue(" :t"), true);
            var itemPlayers = new MenuItem(MenuItemType.Button, new MenuValue("button: ") { Prefix = "<font color=\"#AA33CC\">", Suffix = "<font color=\"#FFFFFF\">" }, players, new MenuValue(" :tail") { Prefix = "<font color=\"#DDAA11\">", Suffix = "<font color=\"#FFFFFF\">" });
            var itemSlider = new MenuItem(MenuItemType.Input, new MenuValue("input: "));

            // 将创建的菜单项添加到主菜单中  
            mainMenu.AddItem(itemOptions);
            mainMenu.AddItem(itemPinwheel);
            mainMenu.AddItem(itemPlayers);
            mainMenu.AddItem(itemSlider);
            mainMenu.AddItem(new MenuItem(MenuItemType.Spacer));

            // 定义一组自定义按钮，每个按钮都有不同的类型和颜色标签  
            var customButtons = new List<MenuValue>
            {
                new ButtonValue("Search", ButtonType.Search) { Prefix = "<font color=\"#AA1133\">", Suffix = "<font color=\"#FFFFFF\">" },
                new ButtonValue("Find", ButtonType.Find),
                new ButtonValue("Select", ButtonType.Select) { Prefix = "<font color=\"#AA1133\">", Suffix = "<font color=\"#FFFFFF\">" }
            };

            // 创建自定义按钮菜单项  
            var itemCustomButtons = new MenuItem(MenuItemType.Button, customButtons);
            // 将自定义按钮菜单项添加到主菜单中  
            mainMenu.AddItem(itemCustomButtons);

            // 设置菜单到控制器，并定义菜单交互逻辑  
            Menu.SetMenu(controller, mainMenu, (buttons, menu, selectedItem) =>
            {
                // 如果按钮不是选择按钮，则直接返回  
                if (buttons != MenuButtons.Select)
                    return;

                // 检查是否选择了自定义按钮菜单项  
                if (menu.Option == 4)
                {
                    // 获取被选择的菜单项的值集合  
                    var menuValues = selectedItem.Values!;
                    // 获取当前选择的菜单值  
                    var selectedValue = menuValues[selectedItem.Option];
                    // 将菜单值转换为ButtonValue类型  
                    var buttonValue = (ButtonValue)selectedValue;

                    // 根据按钮类型执行不同的逻辑  
                    switch (buttonValue.Button)
                    {
                        case ButtonType.Search:
                            Console.WriteLine("Search button clicked");
                            break;
                        case ButtonType.Find:
                            Console.WriteLine("Find button clicked");
                            break;
                        case ButtonType.Select:
                            CustomSelect(controller, new Vector(0, 0, 0));
                            break;
                    }

                    // 另一种逻辑处理方式，直接使用选项索引  
                    switch (selectedItem.Option)
                    {
                        case 0:
                            Console.WriteLine("Search button clicked");
                            break;
                        case 1:
                            Console.WriteLine("Find button clicked");
                            break;
                    }

                    // 另一种逻辑处理方式，直接使用按钮值  
                    switch (buttonValue.Value)
                    {
                        case "Search":
                            Console.WriteLine("Search button clicked");
                            break;
                        case "Find":
                            Console.WriteLine("Find button clicked");
                            break;
                    }
                }
            });
        });

        // 添加一个命令，命名为 "css_test1"，描述为空字符串，执行逻辑定义在 lambda 表达式中  
        AddCommand("css_test1", "", (controller, _) =>
        {
            // 如果控制器为空或者控制器无效，则直接返回，不执行后续代码  
            if (controller == null || !controller.IsValid)
                return;

            // 创建一个新的菜单对象 `dynamicMenu`，菜单名为 "Dynamic Menu"，并设置了前后缀以定义字体大小  
            var dynamicMenu = new MenuBase(new MenuValue("Dynamic Menu") { Prefix = "<font class=\"fontSize-L\">", Suffix = "<font class=\"fontSize-sm\">" });

            // 创建一个新的菜单项 `dynamicItem`，类型为文本，内容为一个动态值（初始为空字符串）  
            var dynamicItem = new MenuItem(MenuItemType.Text, new DynamicValue(""));
            // 将 `dynamicItem` 添加到 `dynamicMenu` 菜单中  
            dynamicMenu.AddItem(dynamicItem);

            // 创建一个保存按钮 `saveButton`，类型为按钮，显示文字为 "Save"  
            var saveButton = new MenuItem(MenuItemType.Button, new List<MenuValue> { new MenuValue("Save") });
            // 将 `saveButton` 添加到 `dynamicMenu` 菜单中  
            dynamicMenu.AddItem(saveButton);

            // 将 `dynamicMenu` 菜单与控制器 `controller` 关联，存储在 `_dynamicMenu` 字典中  
            _dynamicMenu[controller] = dynamicMenu;

            // 设置菜单的逻辑，当菜单被操作时执行此逻辑  
            Menu.SetMenu(controller, dynamicMenu, (buttons, menu, _) =>
            {
                // 如果按下的按钮不是选择按钮，则直接返回  
                if (buttons != MenuButtons.Select)
                    return;

                // 如果选择的菜单项不是第一个（这里假设第一个的索引为0），则直接返回  
                if (menu.Option != 0)
                    return;

                // 当选择的菜单项是第一个时（即 `menu.Option == 0`）  
                if (menu.Option == 0)
                {
                    // 获取第一个菜单项的动态值  
                    var dynamicValue = (DynamicValue)menu.Items[0].Head!;
                    // 输出动态值的内容  
                    Console.WriteLine($"{dynamicValue}");

                    // 调用自定义选择逻辑，传递控制器和动态值的位置  
                    CustomSelect(controller, dynamicValue.Position);
                }
            });
        });
    }

    private void CustomSelect(CCSPlayerController controller, Vector pos)
    {
        // 创建一个子菜单，设置前缀和后缀（注意：前缀不会影响标题，因为它从父菜单继承，但后缀会起作用）  
        var subMenu = new MenuBase(new MenuValue("子菜单") { Prefix = "<font class=\"fontSize-XXXL\">", Suffix = "<font class=\"fontSize-m\">" });

        // 创建一个包含多个选项的列表，每个选项都可以有前缀和后缀来设置样式  
        var options = new List<MenuValue>
        {
            new("o1") { Prefix = "<font color=\"#9900FF\">", Suffix = "<font color=\"#FFFFFF\">" }, // 选项1，紫色文本  
            new("o2"), // 选项2，默认样式  
            new("o3"), // 选项3，默认样式  
            new("o4") { Prefix = "<font color=\"#33AA33\">", Suffix = "<font color=\"#FFFFFF\">" }, // 选项4，绿色文本  
            new("o5") // 选项5，默认样式  
        };

        // 创建一个选择布尔类型的菜单项，用于显示选项列表，并设置其值  
        var itemOptions = new MenuItem(MenuItemType.ChoiceBool, new MenuValue("选项: "), options);
        subMenu.AddItem(itemOptions); // 将选项菜单项添加到子菜单  
        subMenu.AddItem(new MenuItem(MenuItemType.Bool)); // 添加一个布尔类型的菜单项（未知用途，可能是开关）  

        subMenu.AddItem(new MenuItem(MenuItemType.Spacer)); // 添加一个间隔符来分隔菜单项  
        // 添加一个文本类型的菜单项，显示位置信息  
        subMenu.AddItem(new MenuItem(MenuItemType.Text, new MenuValue($"保存: {pos.X} {pos.Y} {pos.Z}")));

        // 将子菜单添加到菜单堆栈中，并设置其操作逻辑  
        Menu.AddMenu(controller, subMenu, (buttons, menu, item) =>
        {
            // 如果按下的按钮不是选择按钮，则不执行任何操作  
            if (buttons != MenuButtons.Select)
                return;

            // 如果选择的菜单项是第一个（选项列表）  
            if (menu.Option == 0)
            {
                // 获取选中的选项值，并打印到控制台  
                var valueItem = item.Values![item.Option];
                Console.WriteLine($"选中: {valueItem.Value} [{item.Option}]");
            }

            // 如果选择的菜单项是第二个（布尔开关）  
            if (menu.Option == 1)
            {
                // 打印布尔值的状态到控制台（这里假设item.Data[0]存储了布尔状态）  
                Console.WriteLine($"布尔值: {item.Data[0]}");
            }
        });
    }
    private void BuildMenu(CCSPlayerController controller)
    {
        // 创建一个主菜单，并设置其前缀和后缀来定义样式  
        var mainMenu = new MenuBase(new MenuValue("主菜单") { Prefix = "<font class=\"fontSize-L\">", Suffix = "<font class=\"fontSize-sm\">" });

        // 创建一个光标样式数组，包含左光标和右光标，并设置它们的样式  
        var cursor = new MenuValue[2]
        {
            new("--> ") { Prefix = "<font color=\"#FFFFFF\">", Suffix = "<font color=\"#FFFFFF\">" }, // 左光标，白色文本  
            new(" <--") { Prefix = "<font color=\"#FFFFFF\">", Suffix = "<font color=\"#FFFFFF\">" }  // 右光标，白色文本  
        };
        mainMenu.Cursor = cursor; // 将光标样式应用到主菜单  

        // 创建一个文本菜单项，并设置其前缀和后缀来定义样式  
        var textItem = new MenuValue("欢迎使用新菜单！");
        textItem.Prefix = "<font color=\"#FF0000\">"; // 红色文本前缀  
        textItem.Suffix = "<font color=\"#FFFFFF\">"; // 白色文本后缀（实际上这里可能不需要，因为通常是用来重置样式的）  

        // 简化后的文本菜单项创建方式（直接在初始化时设置前缀和后缀）  
        // 注意：这里的简化代码并没有真正替换上面的代码，只是展示了另一种创建方式  
        textItem = new MenuValue("欢迎使用新菜单！")
        {
            Prefix = "<font color=\"#FF0000\">",
            Suffix = "<font color=\"#FFFFFF\">"
        };

        // 使用文本菜单项创建一个菜单项对象  
        var simpleTextItem = new MenuItem(MenuItemType.Text, textItem);

        // 将文本菜单项添加到主菜单中  
        mainMenu.AddItem(simpleTextItem);

        // 将主菜单设置到控制器上，并设置一个空的操作逻辑（这里不执行任何操作）  
        Menu.SetMenu(controller, mainMenu, (buttons, menu, item) => { });

        // 菜单库会自动处理菜单的显示和交互逻辑  
        // 使用Tab键（可能是计分板）可以退出菜单，使用Ctrl键（可能是蹲下）可以返回上一级菜单  
    }
    public class PlayerValue(string value, int? id) : MenuValue(value)
    {
        // 定义一个可空的整型属性 Id，初始化为构造函数传入的 id 参数  
        public int? Id { get; set; } = id;

        // 以下两行代码被注释掉了，如果取消注释，它们将定义两个可空的属性  
        // public Player? Player { get; set; }  // 一个可能的 Player 对象  
        // public CCSPlayerController? Controller { get; set; }  // 一个可能的 CCSPlayerController 对象  
    }

    public class ButtonValue(string value, ButtonType button) : MenuValue(value)
    {
        // 定义一个 ButtonType 类型的属性 Button，初始化为构造函数传入的 button 参数  
        public ButtonType Button { get; set; } = button;
    }

    public enum ButtonType
    {
        Search,  // 搜索  
        Find,    // 查找  
        Select   // 选择  
    }

    public class DynamicValue(string value) : MenuValue(value)
    {
        // 定义一个 Vector 类型的属性 Position，初始值为 (0, 0, 0)  
        public Vector Position { get; set; } = new(0, 0, 0);

        // 重写 ToString 方法，返回包含位置信息的字符串  
        public override string ToString()
        {
            return $"{Prefix}x: {Position.X} y: {Position.Y} z: {Position.Z}{Suffix}";
        }
    }
}