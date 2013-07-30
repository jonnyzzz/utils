using System.Threading.Tasks;
using System.Windows.Forms;
using USA.Searcher.Data;

namespace USA.Searcher
{
  public partial class SkyCrawler : Form
  {
    public SkyCrawler()
    {
      InitializeComponent();
      myIE.ScriptErrorsSuppressed = true;
    }

    public async Task<Report> Analyze(Request request)
    {
      return await new SkyScanner(myIE).ScanAsync(request);
    }
  }
}
