% =============================================================================
% DOCUMENT : 03_test_cases.md
% TITLE    : Catalogue des Cas de Test
% FORMAT   : LaTeX / Overleaf
% =============================================================================

```latex
\documentclass[12pt,a4paper]{report}

\usepackage[utf8]{inputenc}
\usepackage[T1]{fontenc}
\usepackage[french]{babel}
\usepackage{geometry}
\usepackage{booktabs}
\usepackage{longtable}
\usepackage{array}
\usepackage{xcolor}
\usepackage{hyperref}
\usepackage{fancyhdr}
\usepackage{titlesec}
\usepackage{enumitem}
\usepackage{tcolorbox}
\usepackage{multirow}

\geometry{margin=2cm}

\definecolor{primary}{RGB}{0,84,166}
\definecolor{secondary}{RGB}{0,150,136}
\definecolor{passfg}{RGB}{56,142,60}
\definecolor{failfg}{RGB}{211,47,47}
\definecolor{nefg}{RGB}{117,117,117}
\definecolor{rowhdr}{RGB}{224,235,248}

\pagestyle{fancy}
\fancyhf{}
\rhead{\textcolor{primary}{\textbf{Catalogue des Cas de Test}}}
\lhead{Bibliothèque – TQL}
\cfoot{\thepage}
\renewcommand{\headrulewidth}{0.4pt}

\titleformat{\chapter}[block]{\Large\bfseries\color{primary}}{}{0em}{}[\titlerule]
\titleformat{\section}{\large\bfseries\color{secondary}}{}{0em}{}
\titleformat{\subsection}{\normalsize\bfseries\color{primary}}{}{0em}{}

% Status macros
\newcommand{\pass}{\textcolor{passfg}{\textbf{Pass}}}
\newcommand{\fail}{\textcolor{failfg}{\textbf{Fail}}}
\newcommand{\ne}{\textcolor{nefg}{Non Exécuté}}

% Test case environment -------------------------------------------------------
\newenvironment{tcblock}[7]{%
  % #1 ID  #2 Titre  #3 Créé par  #4 Revue par  #5 Version  #6 Testeur  #7 Date
  \begin{tcolorbox}[colback=rowhdr!40,colframe=primary,
                    title={\textbf{#1} — #2},
                    fonttitle=\bfseries\small]
  \small
  \begin{tabular}{llllll}
    \textbf{Créé par :} #3 &
    \textbf{Revue par :} #4 &
    \textbf{Version :} #5 &
    \textbf{Testeur :} #6 &
    \textbf{Date :} #7 \\
  \end{tabular}\\[4pt]
}{%
  \end{tcolorbox}
  \vspace{4pt}
}

\begin{document}

% ── Title Page ────────────────────────────────────────────────────────────────
\begin{titlepage}
  \centering
  \vspace*{3cm}
  {\LARGE\bfseries\color{primary}
   Catalogue Complet des Cas de Test\par}
  \vspace{1.5cm}
  \rule{\linewidth}{0.5pt}
  {\large Système de Gestion de Bibliothèque\\[4pt]
   Test et Qualité Logiciel\par}
  \rule{\linewidth}{0.5pt}
  \vspace{2cm}
  \begin{tabular}{ll}
    \textbf{Version :}  & 1.0 \\
    \textbf{Date :}     & \today \\
    \textbf{Niveaux :}  & Unitaire, Intégration, Système, IHM (Selenium) \\
  \end{tabular}
  \vfill
\end{titlepage}

\tableofcontents\newpage

% =============================================================================
\chapter{Vue d'ensemble}
% =============================================================================

\section{Résumé des cas de test}
\begin{longtable}{lll r}
\toprule
\textbf{Préfixe} & \textbf{Niveau} & \textbf{Périmètre} & \textbf{Nb} \\
\midrule
TC-U-BOOK   & Unitaire     & BookController   & 11 \\
TC-U-GENRE  & Unitaire     & GenreController  & 9  \\
TC-U-LOAN   & Unitaire     & LoanController   & 11 \\
TC-U-USER   & Unitaire     & UserController   & 9  \\
TC-I-BOOK   & Intégration  & /api/book        & 7  \\
TC-I-USER   & Intégration  & /api/user        & 7  \\
TC-I-LOAN   & Intégration  & /api/loan        & 8  \\
TC-S        & Système      & Scénarios E2E    & 4  \\
TC-SE-AUTH  & IHM Selenium & Authentification & 5  \\
TC-SE-REG   & IHM Selenium & Inscription      & 3  \\
TC-SE-BOOK  & IHM Selenium & Livres           & 6  \\
TC-SE-LOAN  & IHM Selenium & Prêts            & 6  \\
\midrule
            &              & \textbf{Total}   & \textbf{86} \\
\bottomrule
\caption{Récapitulatif du catalogue}
\end{longtable}

% =============================================================================
\chapter{Tests Unitaires – BookController}
% =============================================================================

\begin{tcblock}{TC-U-BOOK-001}{GetById – ID valide}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\textbf{Préconditions :} Mock \texttt{IBookRepository} configuré pour retourner un \texttt{BookDto} pour l'ID 1.\\[4pt]

\textbf{Données de test :}
\begin{tabular}{ll}
  Entrée & \texttt{id = 1} \\
  Réponse mock & \texttt{new BookDto\{Id=1, Title="Clean Code"\}} \\
\end{tabular}\\[4pt]

\textbf{Étapes :}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule
\textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\
\midrule
1 & Appeler \texttt{GetById(1)} & \texttt{OkObjectResult} retourné & \pass \\
2 & Vérifier le contenu & Body contient \texttt{Id=1, Title="Clean Code"} & \pass \\
\bottomrule
\end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-BOOK-002}{GetById – ID invalide (0 ou négatif)}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\textbf{Préconditions :} Aucune.\\[4pt]
\textbf{Données de test :} \texttt{id = 0} (classe d'équivalence invalide).\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & Appeler \texttt{GetById(0)} & \texttt{BadRequestObjectResult} & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-BOOK-003}{GetById – Livre introuvable}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\textbf{Données de test :} Mock retourne \texttt{null} pour \texttt{id = 99}.\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & Appeler \texttt{GetById(99)} & \texttt{NotFoundResult} & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-BOOK-004}{GetAll – retourne liste paginée}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\textbf{Données de test :} Mock retourne \texttt{PagedResultDto<BookDto>} avec 2 items.\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & Appeler \texttt{GetAll()} & 200 OK, liste non vide & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-BOOK-005}{Create – genre valide}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\textbf{Données de test :} \texttt{GenreExists = true}, \texttt{CreateBookDto} complet.\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & Appeler \texttt{Create(dto)} & \texttt{CreatedAtActionResult} & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-BOOK-006}{Create – genre inexistant}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\textbf{Données de test :} \texttt{GenreExists = false}.\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & Appeler \texttt{Create(dto)} & 400 BadRequest avec message d'erreur & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-BOOK-007}{Update – succès}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\textbf{Données de test :} \texttt{BookExists = true}, dto valide.\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & Appeler \texttt{Update(1, dto)} & 200 OK & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-BOOK-008}{Update – livre introuvable}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\textbf{Données de test :} \texttt{BookExists = false}.\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & Appeler \texttt{Update(99, dto)} & 404 NotFound & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-BOOK-009}{Delete – succès}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\textbf{Données de test :} \texttt{BookExists = true}.\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & Appeler \texttt{Delete(1)} & 204 NoContent & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-BOOK-010}{Delete – livre introuvable}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\textbf{Données de test :} \texttt{BookExists = false}.\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & Appeler \texttt{Delete(99)} & 404 NotFound & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-BOOK-011}{GetAvailableBooks}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\textbf{Données de test :} Mock retourne 2 livres disponibles.\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & GET \texttt{/available} & 200 OK, liste de 2 livres & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

% =============================================================================
\chapter{Tests Unitaires – GenreController}
% =============================================================================

\begin{tcblock}{TC-U-GENRE-001}{GetAll genres – succès}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\textbf{Données :} Mock retourne liste de 3 genres.\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & Appeler \texttt{GetAll()} & 200 OK, TotalCount = 3 & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-GENRE-002}{GetById – trouvé}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{GetById(1)} & 200 OK avec genre & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-GENRE-003}{GetById – introuvable}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{GetById(99)} & 404 NotFound & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-GENRE-004}{Create – nom non dupliqué}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\textbf{Données :} \texttt{GenreNameExists = false}.\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{Create(dto)} & 201 Created & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-GENRE-005}{Create – nom dupliqué}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\textbf{Données :} \texttt{GenreNameExists = true}.\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{Create(dto)} & 409 Conflict & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-GENRE-006}{Update – succès}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{Update(1, dto)} & 200 OK & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-GENRE-007}{Update – introuvable}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{Update(99, dto)} & 404 NotFound & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-GENRE-008}{Delete – succès}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{Delete(1)} & 204 NoContent & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-GENRE-009}{Pagination – paramètres boundary}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\textbf{Données :} \texttt{page=1, size=2}, 5 genres mockés. \textbf{Analyse aux limites.}\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{GetAll(page=1, size=2)} & TotalPages = 3, HasNextPage = true & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

% =============================================================================
\chapter{Tests Unitaires – LoanController}
% =============================================================================

\begin{tcblock}{TC-U-LOAN-001}{GetById – prêt trouvé}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{GetById(1)} & 200 OK avec LoanDto & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-LOAN-002}{CreateLoan – succès}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\textbf{Données :} Validation \texttt{IsValid=true}, livre disponible.\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{CreateLoan(dto)} & 201 Created & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-LOAN-003}{CreateLoan – livre déjà emprunté par l'utilisateur}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\textbf{Données :} \texttt{HasUserBorrowedBook = true}.\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{CreateLoan(dto)} & 400 BadRequest & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-LOAN-004}{CreateLoan – limite de 5 prêts atteinte}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\textbf{Données :} \texttt{Validation.IsValid=false, ErrorMessage="Maximum 5 books"}.\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{CreateLoan(dto)} & 400 BadRequest avec message explicite & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-LOAN-005}{ReturnLoan – succès}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\textbf{Données :} Prêt actif, \texttt{Status="Active"}.\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{ReturnLoan(1)} & 200 OK avec LoanDto mis à jour & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-LOAN-006}{ReturnLoan – prêt déjà retourné}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\textbf{Données :} Mock retourne \texttt{null} (déjà retourné).\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{ReturnLoan(2)} & 400 BadRequest & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-LOAN-007}{CheckAvailability – disponible}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{GetAvailability(1)} & 200 OK, \texttt{IsAvailable=true} & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-LOAN-008}{CheckAvailability – non disponible}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{GetAvailability(2)} & 200 OK, \texttt{IsAvailable=false} & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-LOAN-009}{GetUserActiveLoans – userId vide}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\textbf{Données :} \texttt{userId = ""} (valeur limite invalide).\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{GetUserLoans("")} & 400 BadRequest & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-LOAN-010}{GetUserLoanCount – ID valide}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{GetLoanCount(userId)} & 200 OK, count = 3 & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-LOAN-011}{GetAll loans – liste complète}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{GetAll()} & 200 OK, liste non nulle & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

% =============================================================================
\chapter{Tests Unitaires – UserController}
% =============================================================================

\begin{tcblock}{TC-U-USER-001}{GetAll users}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{GetAll()} & 200 OK, liste de UserDto & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-USER-002}{GetById – trouvé}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{GetById("uid1")} & 200 OK avec UserDto & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-USER-003}{GetById – introuvable}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{GetById("unknown")} & 404 NotFound & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-USER-004}{CreateAdmin – succès}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\textbf{Données :} Mock \texttt{CreateAdminAsync} retourne nouveau UserDto.\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{CreateAdmin(dto)} & 201 Created & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-USER-005}{Update – succès}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{Update("uid1", dto)} & 200 OK & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-USER-006}{Update – introuvable}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{Update("unknown", dto)} & 404 NotFound & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-USER-007}{Delete – succès}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{Delete("uid1")} & 204 NoContent & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-USER-008}{Delete – introuvable}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{Delete("unknown")} & 404 NotFound & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-U-USER-009}{GetById – ID vide (boundary)}{Équipe}{Pairs}{1.0}{xUnit Runner}{\today}
\textbf{Données :} \texttt{id = ""} — valeur limite invalide.\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & \texttt{GetById("")} & 400 ou 404 & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

% =============================================================================
\chapter{Tests d'Intégration}
% =============================================================================

\section{Book API}

\begin{tcblock}{TC-I-BOOK-001}{GET /api/book – liste vide initiale}{Équipe}{Pairs}{1.0}{HttpClient}{\today}
\textbf{Préconditions :} Base de données InMemory vide.\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & GET \texttt{/api/book} & 200 OK, Items=[] & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-I-BOOK-002}{POST /api/book – genre manquant}{Équipe}{Pairs}{1.0}{HttpClient}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & POST avec GenreId inexistant & 400 BadRequest & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-I-BOOK-003}{POST /api/book – succès avec genre valide}{Équipe}{Pairs}{1.0}{HttpClient}{\today}
\textbf{Préconditions :} Genre créé via \texttt{POST /api/genre}.\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & POST /api/genre & 201 & \pass \\
2 & POST /api/book avec genreId & 201 Created, Location header & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-I-BOOK-004}{GET /api/book/\{id\} – introuvable}{Équipe}{Pairs}{1.0}{HttpClient}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & GET /api/book/9999 & 404 NotFound & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-I-BOOK-005}{PUT /api/book/\{id\} – mise à jour}{Équipe}{Pairs}{1.0}{HttpClient}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & Créer un livre & 201 & \pass \\
2 & PUT avec nouveau titre & 200 OK, titre mis à jour & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-I-BOOK-006}{DELETE /api/book/\{id\}}{Équipe}{Pairs}{1.0}{HttpClient}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & DELETE /api/book/\{id\} & 204 NoContent & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-I-BOOK-007}{CRUD complet – Round Trip}{Équipe}{Pairs}{1.0}{HttpClient}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & POST /api/genre & 201 & \pass \\
2 & POST /api/book & 201, Id retourné & \pass \\
3 & GET /api/book/\{id\} & 200 OK & \pass \\
4 & DELETE /api/book/\{id\} & 204 & \pass \\
5 & GET /api/book/\{id\} & 404 NotFound & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\section{User API}

\begin{tcblock}{TC-I-USER-001}{POST /api/user/register – succès}{Équipe}{Pairs}{1.0}{HttpClient}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & POST /register avec données valides & 200 OK & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-I-USER-002}{POST /api/user/register – doublon}{Équipe}{Pairs}{1.0}{HttpClient}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & POST 2e fois même email & 409 Conflict & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-I-USER-003}{POST /api/user/login – succès + JWT}{Équipe}{Pairs}{1.0}{HttpClient}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & POST /login credentials valides & 200 OK, body contient \texttt{token} & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-I-USER-004}{POST /api/user/login – mauvais mot de passe}{Équipe}{Pairs}{1.0}{HttpClient}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & POST /login mauvais pwd & 400 BadRequest & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-I-USER-005}{POST /api/user/login – utilisateur inexistant}{Équipe}{Pairs}{1.0}{HttpClient}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & POST /login email inconnu & 401 Unauthorized & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-I-USER-006}{GET /api/user}{Équipe}{Pairs}{1.0}{HttpClient}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & GET /api/user & 200 OK, liste & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-I-USER-007}{GET /api/user/\{id\} – introuvable}{Équipe}{Pairs}{1.0}{HttpClient}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & GET /api/user/unknownId & 404 NotFound & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\section{Loan API}

\begin{tcblock}{TC-I-LOAN-001}{GET /api/loan – liste vide}{Équipe}{Pairs}{1.0}{HttpClient}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & GET /api/loan & 200 OK & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-I-LOAN-002}{POST /api/loan – livre non disponible}{Équipe}{Pairs}{1.0}{HttpClient}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & POST avec BookId inexistant & 400 BadRequest & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-I-LOAN-003}{DELETE (retour) prêt inexistant}{Équipe}{Pairs}{1.0}{HttpClient}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & PUT /api/loan/9999/return & 400 ou 404 & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-I-LOAN-004}{GET availability – livre disponible}{Équipe}{Pairs}{1.0}{HttpClient}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & GET /api/loan/availability/1 & 200 OK, IsAvailable=true & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-I-LOAN-005}{GET userLoans – userId vide}{Équipe}{Pairs}{1.0}{HttpClient}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & GET /api/loan/user/\%20 & 400 BadRequest & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-I-LOAN-006}{POST – limite 5 prêts}{Équipe}{Pairs}{1.0}{HttpClient}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & Créer 5 prêts & codes 201 pour chacun & \pass \\
2 & Créer 6e prêt & 400 BadRequest & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-I-LOAN-007}{GET loanCount}{Équipe}{Pairs}{1.0}{HttpClient}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & GET /api/loan/count/\{userId\} & 200 OK, count=entier & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-I-LOAN-008}{Flux complet – Register+Login+Emprunt}{Équipe}{Pairs}{1.0}{HttpClient}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & POST /register & 200 OK & \pass \\
2 & POST /login & 200 OK + token JWT & \pass \\
3 & POST /api/genre & 201 & \pass \\
4 & POST /api/book & 201 & \pass \\
5 & POST /api/loan & 201, Status="Active" & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

% =============================================================================
\chapter{Tests Système}
% =============================================================================

\begin{tcblock}{TC-S-001}{Nouveau membre – parcours complet}{Équipe}{Pairs}{1.0}{HttpClient}{\today}
\textbf{Scénario :} Inscription → consultation catalogue → emprunt → retour.\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & Register & 200 OK & \pass \\
2 & Login & Token JWT & \pass \\
3 & GET /api/book & Liste livres & \pass \\
4 & POST /api/loan & 201 Active & \pass \\
5 & PUT /api/loan/\{id\}/return & 200 ReturnDate != null & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-S-002}{Enforcement limite 5 prêts simultanés}{Équipe}{Pairs}{1.0}{HttpClient}{\today}
\textbf{Règle métier :} \texttt{MAX\_BOOKS\_PER\_USER = 5}.\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1--5 & Emprunts successifs & 201 Created & \pass \\
6 & 6e emprunt & 400 BadRequest avec message & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-S-003}{Gestion des genres – flux complet}{Équipe}{Pairs}{1.0}{HttpClient}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & Créer genre & 201 & \pass \\
2 & Ajouter 3 livres & 201 x3 & \pass \\
3 & Mettre à jour genre & 200 & \pass \\
4 & Paginer livres & HasNextPage correct & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-S-004}{Concurrence – même livre, deux utilisateurs}{Équipe}{Pairs}{1.0}{HttpClient}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & User A emprunte livre X & 201 Active & \pass \\
2 & User B emprunte livre X & 400 (non disponible) & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

% =============================================================================
\chapter{Tests IHM – Selenium}
% =============================================================================

\section{Authentification}

\begin{tcblock}{TC-SE-AUTH-001}{Connexion – credentials valides}{Équipe}{Pairs}{1.0}{Selenium}{\today}
\textbf{URL :} \texttt{http://localhost:4200/login} \quad
\textbf{Compte :} \texttt{john.doe@library.com / Client@123}\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & Naviguer vers /login & Page de login affichée & \pass \\
2 & Saisir email & Champ rempli & \pass \\
3 & Saisir mot de passe & Champ rempli & \pass \\
4 & Cliquer Connexion & Redirection vers /home & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-SE-AUTH-002}{Connexion – email invalide}{Équipe}{Pairs}{1.0}{Selenium}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & Saisir email inexistant & — & \pass \\
2 & Cliquer Connexion & Message d'erreur visible & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-SE-AUTH-003}{Connexion – mot de passe incorrect (TC007)}{Équipe}{Pairs}{1.0}{Selenium}{\today}
\textbf{Référence guideline :} Template TC007.\\[4pt]
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & Email valide + mauvais pwd & Formulaire soumis & \pass \\
2 & Vérifier message erreur & Texte d'erreur présent & \pass \\
3 & URL inchangée & Reste sur /login & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-SE-AUTH-004}{Déconnexion}{Équipe}{Pairs}{1.0}{Selenium}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & Se connecter & Connecté & \pass \\
2 & Cliquer Déconnexion & Redirection /login & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-SE-AUTH-005}{Accès page protégée sans token}{Équipe}{Pairs}{1.0}{Selenium}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & GET /books sans connexion & Redirection vers /login & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\section{Inscription}

\begin{tcblock}{TC-SE-REG-001}{Inscription – données valides}{Équipe}{Pairs}{1.0}{Selenium}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & Remplir formulaire & Champs valides & \pass \\
2 & Valider & Message succès & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-SE-REG-002}{Inscription – email déjà pris}{Équipe}{Pairs}{1.0}{Selenium}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & Email existant & — & \pass \\
2 & Soumettre & Message d'erreur affiché & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\begin{tcblock}{TC-SE-REG-003}{Inscription – mot de passe faible}{Équipe}{Pairs}{1.0}{Selenium}{\today}
\begin{longtable}{c p{5cm} p{5cm} c}
\toprule \textbf{\#} & \textbf{Action} & \textbf{Résultat Attendu} & \textbf{Statut} \\ \midrule
1 & Password = "123" & — & \pass \\
2 & Soumettre & Validation côté client / serveur & \pass \\
\bottomrule \end{longtable}
\end{tcblock}

\section{Livres via IHM}
Les cas TC-SE-BOOK-001 à TC-SE-BOOK-006 couvrent :
affichage liste, recherche par titre, ajout admin, modification, suppression, message sans résultat.

\section{Prêts via IHM}
Les cas TC-SE-LOAN-001 à TC-SE-LOAN-006 couvrent :
affichage liste, emprunt d'un livre, retour, limite 5 livres, livres indisponibles grisés, affichage du statut.

\end{document}
```
