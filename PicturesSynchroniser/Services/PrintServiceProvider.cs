using System;
using System.Collections.Generic;
using System.Linq;
using PicturesSynchroniser.Events;
using PicturesSynchroniser.Views;
using Windows.Graphics.Printing;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Printing;

namespace PicturesSynchroniser.Services
{
    public class PrintServiceProvider : Page
    {
        //private Page callingPage;
        //private Type printPageType;
        //private int pageNumber;

        //private double HorizontalPrintMargin = 0.075;
        //private double VerticalPrintMargin = 0.03;

        //private PrintDocument printDocument = null;
        //private IPrintDocumentSource printDocumentSource = null;
        //private List<UIElement> printPreviewPages = new List<UIElement>();

        //private PrintPage firstPage;

        //public event EventHandler<PrintServiceEventArgs> StatusChanged;

        //public PrintServiceProvider()
        //{ 
        //}

        //private Canvas PrintingRoot
        //{
        //    get
        //    {
        //        return this.callingPage.FindName("printingRoot") as Canvas;
        //    }
        //}

        //public void RegisterForPrinting(Page sourcePage, Type printPageType, object viewModel)
        //{ 
        //    this.callingPage = sourcePage;
            
        //    if (PrintingRoot == null)
        //    {
        //        this.OnStatusChanged(new PrintServiceEventArgs("The calling page has no PrintingRoot Canvas."));
        //        return;
        //    }

        //    this.printPageType = printPageType;
        //    this.DataContext = viewModel;
        //    this.PreparePrintContent();

        //    printDocument = new PrintDocument();
        //    printDocumentSource = printDocument.DocumentSource;
        //    printDocument.Paginate += PrintDocument_Paginate;
        //    printDocument.GetPreviewPage += PrintDocument_GetPrintPreviewPage;
        //    printDocument.AddPages += PrintDocument_AddPages;

        //    PrintManager printMan = PrintManager.GetForCurrentView();

        //    try
        //    {
        //        printMan.PrintTaskRequested += PrintManager_PrintTaskRequested;
        //        this.OnStatusChanged(new PrintServiceEventArgs("Registered successfully."));
        //    }
        //    catch (InvalidOperationException)
        //    {
        //        this.OnStatusChanged(new PrintServiceEventArgs("You were already registered."));
        //    }
        //}

        //public void UnregisterForPrinting()
        //{
        //    if (printDocument == null)
        //    {
        //        return;
        //    }

        //    printDocument.Paginate -= PrintDocument_Paginate;
        //    printDocument.GetPreviewPage -= PrintDocument_GetPrintPreviewPage;
        //    printDocument.AddPages -= PrintDocument_AddPages;

        //    PrintManager printManager = PrintManager.GetForCurrentView();
        //    printManager.PrintTaskRequested -= PrintManager_PrintTaskRequested;

        //    PrintingRoot.Children.Clear();
        //}

        //public async void Print()
        //{
        //    if (ApplicationView.Value != ApplicationViewState.Snapped)
        //    {
        //        this.OnStatusChanged(new PrintServiceEventArgs("Opening Print Charm for you."));

        //        try
        //        {
        //            await Windows.Graphics.Printing.PrintManager.ShowPrintUIAsync();
        //            this.OnStatusChanged(new PrintServiceEventArgs(""));
        //        }
        //        catch (Exception)
        //        {
        //            this.OnStatusChanged(new PrintServiceEventArgs("Did you forget to register?"));
        //        }
        //    }
        //    else
        //    {
        //        this.OnStatusChanged(new PrintServiceEventArgs("No printing in snapped mode."));
        //    }
        //}

        //protected virtual void OnStatusChanged(PrintServiceEventArgs e)
        //{
        //    if (this.StatusChanged != null)
        //    {
        //        this.StatusChanged(this, e);
        //    }
        //}

        //private void PrintManager_PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs e)
        //{
        //    PrintTask printTask = null;
        //    printTask = e.Request.CreatePrintTask("U2U Consult MVVM XAML Printing Job.", sourceRequested =>
        //    {
        //        printTask.Completed += async (s, args) =>
        //        {
        //            if (args.Completion == PrintTaskCompletion.Failed)
        //            {
        //                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
        //                {
        //                    this.OnStatusChanged(new PrintServiceEventArgs("Sorry, failed to print."));
        //                });
        //            }
        //        };

        //        sourceRequested.SetSource(printDocumentSource);
        //    });
        //}

        //private void PrintDocument_Paginate(object sender, PaginateEventArgs e)
        //{
        //    printPreviewPages.Clear();
        //    this.pageNumber = 0;
        //    PrintingRoot.Children.Clear();

        //    RichTextBlockOverflow lastRTBOOnPage;
        //    PrintTaskOptions printingOptions = ((PrintTaskOptions)e.PrintTaskOptions);
        //    PrintPageDescription pageDescription = printingOptions.GetPageDescription(0);

        //    lastRTBOOnPage = AddOnePrintPreviewPage(null, pageDescription);
        //    while (lastRTBOOnPage.HasOverflowContent && lastRTBOOnPage.Visibility == Windows.UI.Xaml.Visibility.Visible)
        //    {
        //        lastRTBOOnPage = AddOnePrintPreviewPage(lastRTBOOnPage, pageDescription);
        //    }

        //    PrintDocument printDoc = (PrintDocument)sender;
        //    printDoc.SetPreviewPageCount(printPreviewPages.Count, PreviewPageCountType.Intermediate);
        //}

        //private void PrintDocument_GetPrintPreviewPage(object sender, GetPreviewPageEventArgs e)
        //{
        //    PrintDocument printDoc = (PrintDocument)sender;
        //    printDoc.SetPreviewPage(e.PageNumber, printPreviewPages[e.PageNumber - 1]);
        //}

        //private void PrintDocument_AddPages(object sender, AddPagesEventArgs e)
        //{
        //    for (int i = 0; i < printPreviewPages.Count; i++)
        //    {
        //        printDocument.AddPage(printPreviewPages[i]);
        //    }

        //    PrintDocument printDoc = (PrintDocument)sender;

        //    printDoc.AddPagesComplete();
        //}

        //private RichTextBlockOverflow AddOnePrintPreviewPage(RichTextBlockOverflow lastRTBOAdded, PrintPageDescription printPageDescription)
        //{
        //    FrameworkElement page;

        //    RichTextBlockOverflow textLink;

        //    if (lastRTBOAdded == null)
        //    {
        //        page = firstPage;
        //    }
        //    else
        //    {
        //        page = new PrintPage(lastRTBOAdded);

        //        ((RichTextBlock)page.FindName("textContent")).OverflowContentTarget = null;
        //    }

        //    page.Width = printPageDescription.PageSize.Width;
        //    page.Height = printPageDescription.PageSize.Height;

        //    Grid printableArea = (Grid)page.FindName("printableArea");

        //    double marginWidth = Math.Max(printPageDescription.PageSize.Width - printPageDescription.ImageableRect.Width, printPageDescription.PageSize.Width * HorizontalPrintMargin * 2);
        //    double marginHeight = Math.Max(printPageDescription.PageSize.Height - printPageDescription.ImageableRect.Height, printPageDescription.PageSize.Height * VerticalPrintMargin * 2);

        //    printableArea.Width = page.Width - marginWidth;
        //    printableArea.Height = page.Height - marginHeight;
   
        //    PrintingRoot.Children.Add(page);
        //    PrintingRoot.InvalidateMeasure();
        //    PrintingRoot.UpdateLayout();

        //    textLink = (RichTextBlockOverflow)page.FindName("continuationPageLinkedContainer");

        //    this.pageNumber += 1;
        //    TextBlock pageNumberTextBlock = (TextBlock)page.FindName("pageNumber");
        //    if (pageNumberTextBlock != null)
        //    {
        //        pageNumberTextBlock.Text = string.Format("- {0} -", this.pageNumber);
        //    }

        //    printPreviewPages.Add(page);

        //    return textLink;
        //}

        //private void PreparePrintContent()
        //{
        //    var printPage = Activator.CreateInstance(this.printPageType) as Page;
        //    printPage.DataContext = this.DataContext;

        //    firstPage = new PrintPage();
        //    firstPage.AddContent(new Paragraph());

        //    var printPageRtb = printPage.Content as RichTextBlock;
        //    while (printPageRtb.Blocks.Count > 0)
        //    {
        //        var paragraph = printPageRtb.Blocks.First() as Paragraph;
        //        printPageRtb.Blocks.Remove(paragraph);

        //        var container = paragraph.Inlines[0] as InlineUIContainer;
        //        if (container != null)
        //        {
        //            var measureRtb = new RichTextBlock();
        //            measureRtb.Blocks.Add(paragraph);
        //            PrintingRoot.Children.Clear();
        //            PrintingRoot.Children.Add(measureRtb);
        //            PrintingRoot.InvalidateMeasure();
        //            PrintingRoot.UpdateLayout();

        //            measureRtb.Blocks.Remove(paragraph);
        //            paragraph.LineHeight = measureRtb.ActualHeight;
        //        }

        //        firstPage.AddContent(paragraph);
        //    }
        //    ;

        //    PrintingRoot.Children.Clear();
        //    PrintingRoot.Children.Add(firstPage);
        //}
    }
}