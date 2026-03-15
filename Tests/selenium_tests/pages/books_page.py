# =============================================================================
# FILE    : pages/books_page.py
# PURPOSE : Page Object for the Books management section of the Angular app.
# =============================================================================

from selenium.webdriver.common.by import By
from .base_page import BasePage


class BooksPage(BasePage):
    """
    Represents the /books route – the main book catalogue page.
    """

    PATH = "/books"

    # ── Locators ──────────────────────────────────────────────────────────────
    BOOK_LIST         = (By.CSS_SELECTOR, ".book-list, .books-container, app-book-list")
    BOOK_CARD         = (By.CSS_SELECTOR, ".book-card, .book-item, app-book-card")
    SEARCH_INPUT      = (By.CSS_SELECTOR, "input[placeholder*='Search'], input[placeholder*='search'], .search-input")
    ADD_BOOK_BUTTON   = (By.CSS_SELECTOR, "button.add-book, button[routerlink*='add'], a[routerlink*='add']")
    BOOK_TITLE        = (By.CSS_SELECTOR, ".book-title, h3.title, .card-title")
    LOADING_SPINNER   = (By.CSS_SELECTOR, ".spinner, .loading, app-loading")
    BORROW_BUTTON     = (By.CSS_SELECTOR, "button.borrow-btn, button[class*='borrow']")
    AVAILABLE_BADGE   = (By.CSS_SELECTOR, ".badge-success, .available-badge, .status-available")
    PAGINATION        = (By.CSS_SELECTOR, ".pagination, app-pagination, ngb-pagination")

    # ── Add/Edit Book Form ────────────────────────────────────────────────────
    TITLE_INPUT       = (By.CSS_SELECTOR, "input[formcontrolname='title'], input[name='title']")
    AUTHOR_INPUT      = (By.CSS_SELECTOR, "input[formcontrolname='author'], input[name='author']")
    GENRE_SELECT      = (By.CSS_SELECTOR, "select[formcontrolname='genreId'], select[name='genreId']")
    DATE_INPUT        = (By.CSS_SELECTOR, "input[formcontrolname='publishedDate'], input[type='date']")
    COVER_URL_INPUT   = (By.CSS_SELECTOR, "input[formcontrolname='coverImageUrl'], input[name='coverImageUrl']")
    SAVE_BUTTON       = (By.CSS_SELECTOR, "button[type='submit'], button.save-btn")
    CANCEL_BUTTON     = (By.CSS_SELECTOR, "button.cancel-btn, button[type='button'][class*='cancel']")
    SUCCESS_TOAST     = (By.CSS_SELECTOR, ".toast-success, .alert-success, [class*='success']")
    ERROR_TOAST       = (By.CSS_SELECTOR, ".toast-error, .alert-danger, [class*='error']")
    CONFIRM_DELETE_OK = (By.CSS_SELECTOR, "button.confirm-delete, button.btn-danger[class*='confirm']")

    # ── Actions ───────────────────────────────────────────────────────────────

    def navigate(self, base_url: str):
        self.open(base_url + self.PATH)

    def wait_for_books_to_load(self):
        """Wait until the book list container is present."""
        self.find_element(*self.BOOK_LIST)

    def search_books(self, keyword: str):
        self.type_text(*self.SEARCH_INPUT, keyword)

    def click_add_book(self):
        self.click(*self.ADD_BOOK_BUTTON)

    def fill_book_form(self, title: str, author: str, date: str,
                       cover_url: str = "https://example.com/cover.jpg"):
        """Fill the add/edit book form fields."""
        self.type_text(*self.TITLE_INPUT, title)
        self.type_text(*self.AUTHOR_INPUT, author)
        self.type_text(*self.DATE_INPUT, date)
        self.type_text(*self.COVER_URL_INPUT, cover_url)

    def submit_form(self):
        self.click(*self.SAVE_BUTTON)

    def is_success_shown(self) -> bool:
        return self.is_element_present(*self.SUCCESS_TOAST)

    def is_error_shown(self) -> bool:
        return self.is_element_present(*self.ERROR_TOAST)

    def get_book_count(self) -> int:
        """Return the number of visible book cards."""
        return len(self.driver.find_elements(*self.BOOK_CARD))

    def is_book_visible(self, title: str) -> bool:
        """Return True if a book with the given title is displayed."""
        return self.is_element_present(
            By.XPATH, f"//*[contains(@class,'book-title') or contains(@class,'card-title')][contains(text(),'{title}')]"
        )

    def click_borrow(self, index: int = 0):
        """Click the borrow button of the book at the given index."""
        buttons = self.driver.find_elements(*self.BORROW_BUTTON)
        if buttons and index < len(buttons):
            buttons[index].click()
