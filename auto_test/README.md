# Selenium Test Suite

This folder contains only Selenium UI tests extracted from the main `Tests` area.

## Structure

- `conftest.py` : shared pytest fixtures (WebDriver setup)
- `pytest.ini` : pytest configuration
- `requirements.txt` : Python dependencies
- `pages/` : Page Object Model classes
- `tests/` : Selenium test cases

## Run In VS Code

1. Open this folder in VS Code, or keep the current workspace open.
2. Create and activate a Python virtual environment.
3. Install dependencies:

```bash
pip install -r requirements.txt
```

4. Run tests from terminal:

```bash
pytest
```

After each run, reports are generated automatically in the workspace `reports/` folder:

- `selenium_report.html` (detailed pytest-html report)
- `selenium_report_summary.md` (markdown summary)
- `selenium_report_summary.html` (lightweight HTML summary)
- `selenium_report_summary.xlsx` (Excel summary)

Or use VS Code Testing panel with `pytest` selected.

## Notes

- Default base URL is in `conftest.py`.
- Chrome/Chromedriver are managed using `webdriver-manager`.
