# =============================================================================
# FILE    : conftest.py
# PURPOSE : Shared pytest fixtures – WebDriver setup/teardown and base URL.
#           Uses webdriver-manager to auto-download the correct ChromeDriver.
# NOTE    : AI Assistance Declaration – Selenium POM scaffold and fixture
#           patterns in this file were suggested by GitHub Copilot (Claude).
# =============================================================================

from datetime import datetime
from pathlib import Path

import pytest
from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.chrome.service import Service
from webdriver_manager.chrome import ChromeDriverManager

try:
    from openpyxl import Workbook
except ImportError:  # pragma: no cover - optional dependency at import time
    Workbook = None

# ── Configuration ─────────────────────────────────────────────────────────────

BASE_URL  = "http://localhost:4200"   # Angular dev server
API_URL   = "http://localhost:5118"   # ASP.NET Core API

# Output directory aligned with existing pytest.ini html report path.
REPORTS_DIR = Path(__file__).resolve().parent.parent / "reports"
REPORTS_DIR.mkdir(parents=True, exist_ok=True)


def _safe_text(value):
        """Normalize pytest messages before writing text reports."""
        if value is None:
                return ""
        return str(value).replace("\r", "").strip()


def _build_md_report(run_meta, rows):
        """Build markdown summary for quick sharing in PRs or docs."""
        header = [
                "# Selenium Test Run Report",
                "",
                f"- Run date: {run_meta['run_date']}",
                f"- Total tests: {run_meta['total']}",
                f"- Passed: {run_meta['passed']}",
                f"- Failed: {run_meta['failed']}",
                f"- Skipped: {run_meta['skipped']}",
                "",
                "## Test Details",
                "",
                "| Test | Status | Duration (s) | Message |",
                "|---|---|---:|---|",
        ]

        body = []
        for row in rows:
                message = row["message"].replace("|", "\\|")
                body.append(
                        f"| {row['test']} | {row['status']} | {row['duration']:.3f} | {message} |"
                )

        return "\n".join(header + body) + "\n"


def _build_html_summary(run_meta, rows):
        """Create a lightweight HTML summary in addition to pytest-html output."""
        def esc(text):
                return (
                        str(text)
                        .replace("&", "&amp;")
                        .replace("<", "&lt;")
                        .replace(">", "&gt;")
                )

        row_html = []
        for row in rows:
                status_class = row["status"].lower()
                row_html.append(
                        "<tr>"
                        f"<td>{esc(row['test'])}</td>"
                        f"<td class='{status_class}'>{esc(row['status'])}</td>"
                        f"<td>{row['duration']:.3f}</td>"
                        f"<td>{esc(row['message'])}</td>"
                        "</tr>"
                )

        return f"""<!doctype html>
<html lang=\"en\">
<head>
    <meta charset=\"utf-8\" />
    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />
    <title>Selenium Test Summary</title>
    <style>
        body {{ font-family: Segoe UI, Tahoma, Arial, sans-serif; margin: 24px; color: #1f2937; }}
        .metrics {{ display: flex; gap: 12px; flex-wrap: wrap; margin-bottom: 18px; }}
        .card {{ border: 1px solid #e5e7eb; border-radius: 10px; padding: 10px 14px; background: #f9fafb; }}
        table {{ width: 100%; border-collapse: collapse; }}
        th, td {{ border: 1px solid #e5e7eb; padding: 8px; text-align: left; vertical-align: top; }}
        th {{ background: #f3f4f6; }}
        .passed {{ color: #166534; font-weight: 700; }}
        .failed {{ color: #991b1b; font-weight: 700; }}
        .skipped {{ color: #92400e; font-weight: 700; }}
    </style>
</head>
<body>
    <h1>Selenium Test Run Report</h1>
    <p>Run date: {esc(run_meta['run_date'])}</p>
    <div class=\"metrics\">
        <div class=\"card\"><strong>Total</strong><br />{run_meta['total']}</div>
        <div class=\"card\"><strong>Passed</strong><br />{run_meta['passed']}</div>
        <div class=\"card\"><strong>Failed</strong><br />{run_meta['failed']}</div>
        <div class=\"card\"><strong>Skipped</strong><br />{run_meta['skipped']}</div>
    </div>
    <table>
        <thead>
            <tr>
                <th>Test</th>
                <th>Status</th>
                <th>Duration (s)</th>
                <th>Message</th>
            </tr>
        </thead>
        <tbody>
            {''.join(row_html)}
        </tbody>
    </table>
</body>
</html>
"""


def _write_excel_report(path, rows):
        """Persist a .xlsx report if openpyxl is available."""
        if Workbook is None:
                return False

        wb = Workbook()
        ws = wb.active
        ws.title = "Selenium Results"
        ws.append(["Test", "Status", "Duration (s)", "Message"])
        for row in rows:
                ws.append([row["test"], row["status"], float(f"{row['duration']:.3f}"), row["message"]])

        wb.save(path)
        return True


def pytest_addoption(parser):
    """Allow switching browser mode from the command line."""
    parser.addoption(
        "--headless",
        action="store_true",
        default=False,
        help="Run browser in headless mode (for CI).",
    )


def pytest_configure(config):
    """Session-level store for per-test outcome rows used in report generation."""
    config._result_rows = {}


@pytest.hookimpl(hookwrapper=True)
def pytest_runtest_makereport(item, call):
    """Capture final outcome for each test item from setup/call phases."""
    outcome = yield
    report = outcome.get_result()

    if report.when not in ("setup", "call"):
        return

    rows = item.config._result_rows
    nodeid = report.nodeid

    status = "PASSED"
    if report.failed:
        status = "FAILED"
    elif report.skipped:
        status = "SKIPPED"

    message = ""
    if report.failed and hasattr(report, "longreprtext"):
        message = _safe_text(report.longreprtext).split("\n")[-1]
    elif report.skipped and hasattr(report, "longrepr") and isinstance(report.longrepr, tuple):
        message = _safe_text(report.longrepr[2])

    # Prefer call phase when available, but keep setup failures.
    previous = rows.get(nodeid)
    if previous and report.when == "setup" and previous.get("phase") == "call":
        return

    rows[nodeid] = {
        "test": nodeid,
        "status": status,
        "duration": float(getattr(report, "duration", 0.0)),
        "message": message,
        "phase": report.when,
    }


def pytest_sessionfinish(session, exitstatus):
    """Generate markdown/html/excel summaries after every pytest run."""
    _ = exitstatus  # Reserved for future use.

    result_rows = [
        {
            "test": row["test"],
            "status": row["status"],
            "duration": row["duration"],
            "message": row["message"],
        }
        for row in sorted(session.config._result_rows.values(), key=lambda item: item["test"])
    ]

    total = len(result_rows)
    passed = sum(1 for item in result_rows if item["status"] == "PASSED")
    failed = sum(1 for item in result_rows if item["status"] == "FAILED")
    skipped = sum(1 for item in result_rows if item["status"] == "SKIPPED")

    run_meta = {
        "run_date": datetime.now().strftime("%Y-%m-%d %H:%M:%S"),
        "total": total,
        "passed": passed,
        "failed": failed,
        "skipped": skipped,
    }

    md_path = REPORTS_DIR / "selenium_report_summary.md"
    html_summary_path = REPORTS_DIR / "selenium_report_summary.html"
    xlsx_path = REPORTS_DIR / "selenium_report_summary.xlsx"

    md_path.write_text(_build_md_report(run_meta, result_rows), encoding="utf-8")
    html_summary_path.write_text(_build_html_summary(run_meta, result_rows), encoding="utf-8")
    excel_ok = _write_excel_report(xlsx_path, result_rows)

    tr = session.config.pluginmanager.get_plugin("terminalreporter")
    if tr:
        tr.write_sep("=", f"Markdown summary: {md_path}")
        tr.write_sep("=", f"HTML summary: {html_summary_path}")
        if excel_ok:
            tr.write_sep("=", f"Excel summary: {xlsx_path}")
        else:
            tr.write_sep("=", "Excel summary skipped (openpyxl not installed)")


def _build_chrome_options(headless=False):
    """Centralized Chrome options used by all fixtures."""
    options = Options()
    if headless:
        options.add_argument("--headless=new")
    options.add_argument("--no-sandbox")
    options.add_argument("--disable-dev-shm-usage")
    options.add_argument("--window-size=1920,1080")
    options.add_argument("--disable-gpu")
    return options


# ── WebDriver Fixture ─────────────────────────────────────────────────────────

@pytest.fixture(scope="session")
def driver(request):
    """
    Session-scoped Chrome WebDriver.
    Runs once for the entire test session (faster than per-function).
    """
    options = _build_chrome_options(headless=request.config.getoption("--headless"))

    service = Service(ChromeDriverManager().install())
    drv = webdriver.Chrome(service=service, options=options)
    drv.implicitly_wait(10)   # Global implicit wait: 10 seconds
    yield drv
    drv.quit()


@pytest.fixture(scope="function")
def fresh_driver(request):
    """
    Function-scoped Chrome WebDriver.
    Used by tests that require a clean browser state (e.g. login/logout tests).
    """
    options = _build_chrome_options(headless=request.config.getoption("--headless"))

    service = Service(ChromeDriverManager().install())
    drv = webdriver.Chrome(service=service, options=options)
    drv.implicitly_wait(10)
    yield drv
    drv.quit()
