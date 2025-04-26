using Microsoft.AspNetCore.Mvc;
using DTech.DAO;

namespace DTech.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FeedbacksController(
        FeedbackDAO feedbackDAO
    ) : Controller
    {

        // GET: Admin/Feedbacks
        public async Task<IActionResult> Index()
        {
            return View(await feedbackDAO.GetListAsync());
        }

        // GET: Admin/Feedbacks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await feedbackDAO.GetByIdAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // GET: Admin/Feedbacks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await feedbackDAO.GetByIdAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // POST: Admin/Feedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feedback = await feedbackDAO.GetByIdAsync(id);
            if (feedback != null)
            {
                await feedbackDAO.DeleteAsync(feedback);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
