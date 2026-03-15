# =============================================================================
# FILE    : pages/loans_page.py
# PURPOSE : Page Object for the Loans management section of the Angular app.
# =============================================================================

from selenium.webdriver.common.by import By

from .base_page import BasePage


class LoansPage(BasePage):
    """
    Represents the /admin/orders route – loan tracking for admins.
    """

    PATH = "/admin/orders"

    # ── Locators ──────────────────────────────────────────────────────────────
    LOANS_TABLE       = (By.CSS_SELECTOR, "table, .glass-panel")
    LOAN_ROW          = (By.CSS_SELECTOR, "tbody tr")
    STATUS_CELL       = (By.CSS_SELECTOR, ".loan-status, td.status, [class*='status']")
    RETURN_BUTTON     = (By.XPATH, "//button[contains(., 'Return')]")
    OVERDUE_BADGE     = (By.CSS_SELECTOR, ".badge-danger, .overdue-badge, [class*='overdue'], .text-rose-400, .text-rose-600")
    ACTIVE_BADGE      = (By.CSS_SELECTOR, ".badge-success, [class*='active-badge']")
    LOAN_BOOK_TITLE   = (By.CSS_SELECTOR, ".loan-book-title, td.book-title")
    FILTER_STATUS     = (By.CSS_SELECTOR, "select[name='status'], select[formcontrolname='status']")
    LOAN_COUNT        = (By.CSS_SELECTOR, ".loan-count, .total-loans, .badge-count")
    EMPTY_MESSAGE     = (By.XPATH, "//*[contains(., 'No loans found') or contains(., 'No loans to show')]")
    SUCCESS_TOAST     = (By.CSS_SELECTOR, ".toast-success, .alert-success")
    ERROR_TOAST       = (By.CSS_SELECTOR, ".toast-error, .alert-danger")

    def navigate(self, base_url: str):
        self.open(base_url + self.PATH)

    def wait_for_loans_to_load(self):
        """Wait until the loans container is present."""
        self.find_element(*self.LOANS_TABLE)

    def get_loan_count(self) -> int:
        """Return the number of visible loan rows."""
        return len(self.driver.find_elements(*self.LOAN_ROW))

    def click_return(self, index: int = 0):
        """Click the return button for the loan at the given index."""
        buttons = self.driver.find_elements(*self.RETURN_BUTTON)
        if buttons and index < len(buttons):
            buttons[index].click()

    def is_success_shown(self) -> bool:
        return self.is_element_present(*self.SUCCESS_TOAST)

    def is_overdue_present(self) -> bool:
        return self.is_element_present(*self.OVERDUE_BADGE)

    def has_no_loans(self) -> bool:
        return self.is_element_present(*self.EMPTY_MESSAGE)
