using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using MyBlog.Models.ViewModels.NavigationViewModels;

namespace MyBlog.TagHelpers
{
    // <page-link page-vm="Model.PageVM!" (1)
    //        page-action="Index" (2)
    //        page-url-categoryId="@(Model.FilterVM.CategoryId)"
    //        page-url-search="@(Model.FilterVM.Search!)"
    //        page-url-sortOrder="@(Model.SortVM.Current)">
    // </page-link>

    //Здесь прамо в коде будет много комментариев
    // Выше для примера закомментированый tag-helper,
    // вызываемый со страницы .cshtml.
    // Можете сравнивать с ним параметры и свойства,
    // которые в него передаются.

    public class PageLinkTagHelper : TagHelper // Наследуем от класса TagHelper
    {
        // IUrlHelperFactory - сервис который используется для создания ссылки
        // и который мы можем получить в конструкторе.
        private IUrlHelperFactory urlHelperFactory;

        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            // получаем IUrlHelperFactory
            urlHelperFactory = helperFactory;
        }


        // Для создания ссылки используется объект IUrlHelper,
        // а для его получения нам нужен контекст представления,
        // в котором вызывается tag-хелпер.
        // Получить контекст представления мы можем
        // через внедрение зависимости через атрибуты.
        // В частности, чтобы получить контекст представления
        // над свойством ставится атрибут ViewContext.
        [ViewContext]
        // Чтобы избежать привязки к атрибутам тега,
        // к свойству также применяется атрибут HtmlAttributeNotBound.
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = default!;

        // Наша PageVM, в этом случае играет роль свойства,
        // получаемого при передаче папаметра тег-хелперу
        public PageVM PageVM { get; set; } = default!;

        // Свойство PageAction указывает на метод контроллера,
        // на который будет создаваться ссылка (в нашем примере - Index)
        public string PageAction { get; set; } = default!;


        // Это свойство представляет словарь Dictionary<string, object>,
        // в котором каждой строке будет сопоставлен некоторый объект.
        //  Атрибут [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        //  указывает, что при применении хелпера в представлении мы сможем передать ему
        //  некоторые значения через атрибуты с префиксом "page-url-".
        // Смотреть в самон начале закоментированыый tag-helper
        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; } = new Dictionary<string, object>();


        // Единственный переопределенный метод Process[Async]
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Получаем объект IUrlHelper для создания ссылки,
            // передав ViewContext, описаный выше
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);

            // родительский элемент - div
            output.TagName = "div";

            // в нем будет ul
            TagBuilder tag = new TagBuilder("ul");
            // бутстрапим классами bootstrap
            tag.AddCssClass("pagination justify-content-center");

            // Начальный элемент "<<" и немного логики по его отображению
            TagBuilder back = CreateTag(" << ", urlHelper); // Метод создания элемента
            if (!PageVM.HasPreviousPage)
            {
                back.AddCssClass("disabled");
            }
            tag.InnerHtml.AppendHtml(back);

            ////////////////////////////////////////////////////
            // Формируем ниже список по типу "<< 1 5 6 7 8 (current) 9 10 11 25 >>"
            // То есть 3 слева и 3 справа от текущей, если они есть
            // Здесь формируем ДО текущей страницы
            for (int i = 1; i <= PageVM.PageNumber; i++)
            {
                if ((PageVM.PageNumber - 3 <= i) || (i == 1) || (PageVM.PageNumber == i))
                {
                    TagBuilder Item = CreateTag(i.ToString(), urlHelper);
                    tag.InnerHtml.AppendHtml(Item);
                }
            }

            // Здесь формируем ПОСЛЕ текущей страницы
            for (int i = PageVM.PageNumber + 1; i <= PageVM.TotalPages; i++)
            {
                if ((PageVM.PageNumber + 3 >= i) || (i == PageVM.TotalPages) || (PageVM.PageNumber == i))
                {
                    TagBuilder nextItem = CreateTag(i.ToString(), urlHelper);
                    tag.InnerHtml.AppendHtml(nextItem);
                }
            }
            /////////////////////////////////////////////////////

            // Конечный элемент >>
            TagBuilder forward = CreateTag(" >> ", urlHelper);
            if (!PageVM.HasNextPage)
            {
                forward.AddCssClass("disabled");
            }
            tag.InnerHtml.AppendHtml(forward);

            output.Content.AppendHtml(tag);
        }


        // Метод создания элемента
        // pageNumber переведен в строки, потому что есть "<<" и ">>"
        TagBuilder CreateTag(string pageNumber, IUrlHelper urlHelper)
        {
            // создаем li
            TagBuilder item = new TagBuilder("li");
            // создаем ссылку
            TagBuilder link = new TagBuilder("a");

            // если выше из for номер текущей страницы -
            // стилизуем ее классом bootstrap - active
            if (pageNumber == this.PageVM.PageNumber.ToString())
            {
                item.AddCssClass("active");
            }
            else
            {
                // если нажали на << то текущую страницу на 1 назад
                if (pageNumber == " << ")
                {
                    PageUrlValues["page"] = PageVM.PageNumber - 1;
                }
                // если >> то на 1 вперед
                else if (pageNumber == " >> ")
                {
                    PageUrlValues["page"] = PageVM.PageNumber + 1;
                }
                // или нажали на конкретную страницу
                else
                {
                    PageUrlValues["page"] = pageNumber;
                }

                // в атрибут href добавляем action (Index) и значения
                // из словаря (смотреть в самон начале закоментированыый tag-helper)
                // выйдет минимум что-то типа как ниже
                // "<a href=\"/Posts?categoryId=0&amp;sortOrder=TitleAsc&amp;page=4\"></a>"
                link.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
            }
            // бутстрапим
            item.AddCssClass("page-item");
            link.AddCssClass("page-link link-success fw-bold border-bottom border-5 border-info");
            // в ссылку добавляем номер страницы или же << >>
            link.InnerHtml.Append(pageNumber.ToString());
            // в li добавляем ссылку
            item.InnerHtml.AppendHtml(link);
            // возвращаем
            return item;
        }
    }
}
