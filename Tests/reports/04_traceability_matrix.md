% =============================================================================
% DOCUMENT : 04_traceability_matrix.md
% TITLE    : Matrice de Traçabilité
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
\usepackage{multirow}
\usepackage{tcolorbox}
\usepackage{rotating}
\usepackage{pdflscape}

\geometry{margin=2cm}

\definecolor{primary}{RGB}{0,84,166}
\definecolor{secondary}{RGB}{0,150,136}
\definecolor{covered}{RGB}{200,230,201}
\definecolor{partial}{RGB}{255,236,179}
\definecolor{missing}{RGB}{255,205,210}
\definecolor{rowalt}{RGB}{232,244,253}

\pagestyle{fancy}
\fancyhf{}
\rhead{\textcolor{primary}{\textbf{Matrice de Traçabilité}}}
\lhead{Gestion Bibliothèque – TQL}
\cfoot{\thepage}
\renewcommand{\headrulewidth}{0.4pt}

\titleformat{\chapter}[block]{\Large\bfseries\color{primary}}{}{0em}{}[\titlerule]
\titleformat{\section}{\large\bfseries\color{secondary}}{}{0em}{}

\newcommand{\covered}{\cellcolor{covered}}
\newcommand{\partial}{\cellcolor{partial}}
\newcommand{\missing}{\cellcolor{missing}}

\begin{document}

% ── Title Page ────────────────────────────────────────────────────────────────
\begin{titlepage}
  \centering
  \vspace*{3cm}
  {\LARGE\bfseries\color{primary}
   Matrice de Traçabilité\\[0.4em]
   Exigences $\rightarrow$ Cas de Test $\rightarrow$ Résultats\par}
  \vspace{1.5cm}
  \rule{\linewidth}{0.5pt}
  {\large Système de Gestion de Bibliothèque\\[4pt]
   Test et Qualité Logiciel\par}
  \rule{\linewidth}{0.5pt}
  \vspace{2cm}
  \begin{tabular}{ll}
    \textbf{Version :} & 1.0 \\
    \textbf{Date :}    & \today \\
  \end{tabular}
  \vfill
\end{titlepage}

\tableofcontents\newpage

% =============================================================================
\chapter{Légende et Conventions}
% =============================================================================

\begin{tcolorbox}[colframe=primary, colback=rowalt!40, title=Légende de couverture]
\begin{tabular}{lll}
  \cellcolor{covered}\quad & \textbf{Couverte} & Exigence testée par ≥ 1 cas de test \\[4pt]
  \cellcolor{partial}\quad & \textbf{Partielle} & Exigence testée mais scénario non exhaustif \\[4pt]
  \cellcolor{missing}\quad & \textbf{Manquante} & Exigence non couverte (hors-scope du projet) \\
\end{tabular}
\end{tcolorbox}

\bigskip

\textbf{Niveaux de test :}
\begin{itemize}
  \item \textbf{U} = Unitaire (xUnit + Moq)
  \item \textbf{I} = Intégration (WebApplicationFactory + EF InMemory)
  \item \textbf{S} = Système (E2E API)
  \item \textbf{SE} = IHM Selenium (pytest POM)
\end{itemize}

\textbf{Statuts de résultat :}
\begin{itemize}
  \item \textcolor{secondary}{\textbf{PASS}} — Test exécuté avec succès
  \item \textcolor{danger}{\textbf{FAIL}} — Test échoué (si applicable)
  \item \textbf{NE} — Non Exécuté (test défini, environnement non lancé)
\end{itemize}

% =============================================================================
\chapter{Catalogue des Exigences}
% =============================================================================

\begin{longtable}{p{1.5cm} p{4cm} p{7cm} p{2.5cm}}
\toprule
\textbf{ID Req.} & \textbf{Catégorie} & \textbf{Description} & \textbf{Priorité} \\
\midrule
\endhead

REQ-01 & Authentification & L'utilisateur peut s'inscrire avec email + mot de passe & Haute \\[4pt]
REQ-02 & Authentification & L'utilisateur peut se connecter et obtenir un JWT & Haute \\[4pt]
REQ-03 & Authentification & Accès refusé sans token valide & Haute \\[4pt]
REQ-04 & Livres & Afficher la liste paginée des livres & Haute \\[4pt]
REQ-05 & Livres & Créer un livre avec genre obligatoire & Haute \\[4pt]
REQ-06 & Livres & Modifier un livre existant & Moyenne \\[4pt]
REQ-07 & Livres & Supprimer un livre & Moyenne \\[4pt]
REQ-08 & Livres & Consulter les livres disponibles & Haute \\[4pt]
REQ-09 & Genres & Créer / modifier / supprimer des genres & Moyenne \\[4pt]
REQ-10 & Genres & Pagination des genres & Basse \\[4pt]
REQ-11 & Genres & Nom de genre unique & Haute \\[4pt]
REQ-12 & Prêts & Emprunter un livre disponible & Haute \\[4pt]
REQ-13 & Prêts & Limite de 5 prêts actifs par utilisateur & Haute \\[4pt]
REQ-14 & Prêts & Retourner un livre emprunté & Haute \\[4pt]
REQ-15 & Prêts & Un livre ne peut être emprunté qu'une seule fois simultanément & Haute \\[4pt]
REQ-16 & Prêts & Consulter ses prêts actifs & Haute \\[4pt]
REQ-17 & Utilisateurs & Gestion CRUD des utilisateurs (admin) & Moyenne \\[4pt]
REQ-18 & IHM & Interface Angular fonctionnelle pour les flux métier & Haute \\[4pt]

\bottomrule
\caption{Catalogue des exigences du système}
\end{longtable}

% =============================================================================
\chapter{Matrice Exigences $\times$ Cas de Test}
% =============================================================================

\begin{longtable}{p{1.4cm} p{4.5cm} p{6.5cm} p{2cm}}
\toprule
\textbf{ID Req.} & \textbf{Description courte} & \textbf{Cas de test associés} & \textbf{Couverture} \\
\midrule
\endhead

\covered REQ-01
  & Inscription utilisateur
  & TC-I-USER-001, TC-I-USER-002, TC-SE-REG-001, TC-SE-REG-002, TC-SE-REG-003
  & \covered Couverte \\[6pt]

\covered REQ-02
  & Connexion + JWT
  & TC-I-USER-003, TC-I-USER-004, TC-I-USER-005, TC-SE-AUTH-001, TC-SE-AUTH-002, TC-SE-AUTH-003
  & \covered Couverte \\[6pt]

\covered REQ-03
  & Accès sans token
  & TC-SE-AUTH-005, TC-I-LOAN-008 (token requis)
  & \partial Partielle \\[6pt]

\covered REQ-04
  & Liste livres paginée
  & TC-U-BOOK-004, TC-I-BOOK-001, TC-I-BOOK-003, TC-SE-BOOK-001
  & \covered Couverte \\[6pt]

\covered REQ-05
  & Créer livre + genre obligatoire
  & TC-U-BOOK-005, TC-U-BOOK-006, TC-I-BOOK-002, TC-I-BOOK-003, TC-SE-BOOK-003
  & \covered Couverte \\[6pt]

\covered REQ-06
  & Modifier livre
  & TC-U-BOOK-007, TC-U-BOOK-008, TC-I-BOOK-005, TC-SE-BOOK-004
  & \covered Couverte \\[6pt]

\covered REQ-07
  & Supprimer livre
  & TC-U-BOOK-009, TC-U-BOOK-010, TC-I-BOOK-006, TC-I-BOOK-007(étape 4), TC-SE-BOOK-005
  & \covered Couverte \\[6pt]

\covered REQ-08
  & Livres disponibles
  & TC-U-BOOK-011, TC-U-LOAN-007, TC-U-LOAN-008, TC-SE-LOAN-005
  & \covered Couverte \\[6pt]

\covered REQ-09
  & CRUD genres
  & TC-U-GENRE-004, TC-U-GENRE-006, TC-U-GENRE-008, TC-I-BOOK-003(genre préalable), TC-S-003
  & \covered Couverte \\[6pt]

\covered REQ-10
  & Pagination genres
  & TC-U-GENRE-009, TC-U-GENRE-001
  & \partial Partielle \\[6pt]

\covered REQ-11
  & Nom genre unique
  & TC-U-GENRE-005, TC-I-BOOK-002
  & \covered Couverte \\[6pt]

\covered REQ-12
  & Emprunter livre
  & TC-U-LOAN-002, TC-I-LOAN-008, TC-S-001, TC-SE-LOAN-002
  & \covered Couverte \\[6pt]

\covered REQ-13
  & Limite 5 prêts
  & TC-U-LOAN-004, TC-I-LOAN-006, TC-S-002, TC-SE-LOAN-004
  & \covered Couverte \\[6pt]

\covered REQ-14
  & Retourner livre
  & TC-U-LOAN-005, TC-U-LOAN-006, TC-I-LOAN-003, TC-S-001(étape 5), TC-SE-LOAN-003
  & \covered Couverte \\[6pt]

\covered REQ-15
  & Un livre = un emprunt
  & TC-U-LOAN-003, TC-S-004
  & \covered Couverte \\[6pt]

\covered REQ-16
  & Prêts actifs utilisateur
  & TC-U-LOAN-009, TC-U-LOAN-010, TC-I-LOAN-005, TC-SE-LOAN-001
  & \covered Couverte \\[6pt]

\covered REQ-17
  & CRUD utilisateurs
  & TC-U-USER-001 à TC-U-USER-009, TC-I-USER-006, TC-I-USER-007
  & \covered Couverte \\[6pt]

\covered REQ-18
  & IHM Angular
  & TC-SE-AUTH-001 à TC-SE-LOAN-006 (ensemble Selenium)
  & \partial Partielle \\[6pt]

\bottomrule
\caption{Matrice de traçabilité – Exigences vers Cas de Test}
\end{longtable}

% =============================================================================
\chapter{Matrice Cas de Test $\times$ Résultats}
% =============================================================================

\begin{longtable}{p{2.8cm} p{5.5cm} p{2cm} p{2cm} p{1.5cm}}
\toprule
\textbf{ID TC} & \textbf{Titre} & \textbf{Niveau} & \textbf{Req. couverte} & \textbf{Résultat} \\
\midrule
\endhead

TC-U-BOOK-001 & GetById ID valide              & U  & REQ-04 & \textcolor{secondary}{PASS} \\
TC-U-BOOK-002 & GetById ID invalide            & U  & REQ-04 & \textcolor{secondary}{PASS} \\
TC-U-BOOK-003 & GetById introuvable            & U  & REQ-04 & \textcolor{secondary}{PASS} \\
TC-U-BOOK-004 & GetAll paginé                  & U  & REQ-04 & \textcolor{secondary}{PASS} \\
TC-U-BOOK-005 & Create genre valide            & U  & REQ-05 & \textcolor{secondary}{PASS} \\
TC-U-BOOK-006 & Create genre inexistant        & U  & REQ-05 & \textcolor{secondary}{PASS} \\
TC-U-BOOK-007 & Update succès                  & U  & REQ-06 & \textcolor{secondary}{PASS} \\
TC-U-BOOK-008 & Update introuvable             & U  & REQ-06 & \textcolor{secondary}{PASS} \\
TC-U-BOOK-009 & Delete succès                  & U  & REQ-07 & \textcolor{secondary}{PASS} \\
TC-U-BOOK-010 & Delete introuvable             & U  & REQ-07 & \textcolor{secondary}{PASS} \\
TC-U-BOOK-011 & GetAvailable                   & U  & REQ-08 & \textcolor{secondary}{PASS} \\
\midrule
TC-U-GENRE-001 & GetAll genres                 & U  & REQ-04,REQ-10 & \textcolor{secondary}{PASS} \\
TC-U-GENRE-002 & GetById trouvé               & U  & REQ-09 & \textcolor{secondary}{PASS} \\
TC-U-GENRE-003 & GetById introuvable           & U  & REQ-09 & \textcolor{secondary}{PASS} \\
TC-U-GENRE-004 & Create non dupliqué           & U  & REQ-09,REQ-11 & \textcolor{secondary}{PASS} \\
TC-U-GENRE-005 & Create dupliqué              & U  & REQ-11 & \textcolor{secondary}{PASS} \\
TC-U-GENRE-006 & Update succès                 & U  & REQ-09 & \textcolor{secondary}{PASS} \\
TC-U-GENRE-007 & Update introuvable            & U  & REQ-09 & \textcolor{secondary}{PASS} \\
TC-U-GENRE-008 & Delete succès                 & U  & REQ-09 & \textcolor{secondary}{PASS} \\
TC-U-GENRE-009 & Pagination boundary           & U  & REQ-10 & \textcolor{secondary}{PASS} \\
\midrule
TC-U-LOAN-001 & GetById prêt trouvé            & U  & REQ-16 & \textcolor{secondary}{PASS} \\
TC-U-LOAN-002 & CreateLoan succès              & U  & REQ-12 & \textcolor{secondary}{PASS} \\
TC-U-LOAN-003 & CreateLoan déjà emprunté       & U  & REQ-15 & \textcolor{secondary}{PASS} \\
TC-U-LOAN-004 & CreateLoan limite atteinte     & U  & REQ-13 & \textcolor{secondary}{PASS} \\
TC-U-LOAN-005 & ReturnLoan succès              & U  & REQ-14 & \textcolor{secondary}{PASS} \\
TC-U-LOAN-006 & ReturnLoan déjà retourné       & U  & REQ-14 & \textcolor{secondary}{PASS} \\
TC-U-LOAN-007 & CheckAvailability dispo        & U  & REQ-08 & \textcolor{secondary}{PASS} \\
TC-U-LOAN-008 & CheckAvailability non dispo    & U  & REQ-08 & \textcolor{secondary}{PASS} \\
TC-U-LOAN-009 & GetUserLoans userId vide       & U  & REQ-16 & \textcolor{secondary}{PASS} \\
TC-U-LOAN-010 & GetUserLoanCount               & U  & REQ-16 & \textcolor{secondary}{PASS} \\
TC-U-LOAN-011 & GetAll loans                   & U  & REQ-16 & \textcolor{secondary}{PASS} \\
\midrule
TC-U-USER-001 & GetAll users                   & U  & REQ-17 & \textcolor{secondary}{PASS} \\
TC-U-USER-002 & GetById trouvé                 & U  & REQ-17 & \textcolor{secondary}{PASS} \\
TC-U-USER-003 & GetById introuvable            & U  & REQ-17 & \textcolor{secondary}{PASS} \\
TC-U-USER-004 & CreateAdmin succès             & U  & REQ-17 & \textcolor{secondary}{PASS} \\
TC-U-USER-005 & Update succès                  & U  & REQ-17 & \textcolor{secondary}{PASS} \\
TC-U-USER-006 & Update introuvable             & U  & REQ-17 & \textcolor{secondary}{PASS} \\
TC-U-USER-007 & Delete succès                  & U  & REQ-17 & \textcolor{secondary}{PASS} \\
TC-U-USER-008 & Delete introuvable             & U  & REQ-17 & \textcolor{secondary}{PASS} \\
TC-U-USER-009 & GetById ID vide               & U  & REQ-17 & \textcolor{secondary}{PASS} \\
\midrule
TC-I-BOOK-001 & GET liste vide                 & I  & REQ-04 & \textcolor{secondary}{PASS} \\
TC-I-BOOK-002 & POST genre manquant            & I  & REQ-05 & \textcolor{secondary}{PASS} \\
TC-I-BOOK-003 & POST succès                    & I  & REQ-04,REQ-05 & \textcolor{secondary}{PASS} \\
TC-I-BOOK-004 & GET introuvable                & I  & REQ-04 & \textcolor{secondary}{PASS} \\
TC-I-BOOK-005 & PUT mise à jour                & I  & REQ-06 & \textcolor{secondary}{PASS} \\
TC-I-BOOK-006 & DELETE                         & I  & REQ-07 & \textcolor{secondary}{PASS} \\
TC-I-BOOK-007 & CRUD Round Trip                & I  & REQ-04--REQ-07 & \textcolor{secondary}{PASS} \\
\midrule
TC-I-USER-001 & Register succès                & I  & REQ-01 & \textcolor{secondary}{PASS} \\
TC-I-USER-002 & Register doublon               & I  & REQ-01 & \textcolor{secondary}{PASS} \\
TC-I-USER-003 & Login + JWT                    & I  & REQ-02 & \textcolor{secondary}{PASS} \\
TC-I-USER-004 & Login mauvais pwd              & I  & REQ-02 & \textcolor{secondary}{PASS} \\
TC-I-USER-005 & Login inexistant               & I  & REQ-02 & \textcolor{secondary}{PASS} \\
TC-I-USER-006 & GET /users                     & I  & REQ-17 & \textcolor{secondary}{PASS} \\
TC-I-USER-007 & GET user introuvable           & I  & REQ-17 & \textcolor{secondary}{PASS} \\
\midrule
TC-I-LOAN-001 & GET vide                       & I  & REQ-16 & \textcolor{secondary}{PASS} \\
TC-I-LOAN-002 & POST non disponible            & I  & REQ-12 & \textcolor{secondary}{PASS} \\
TC-I-LOAN-003 & Retour inexistant              & I  & REQ-14 & \textcolor{secondary}{PASS} \\
TC-I-LOAN-004 & GET availability               & I  & REQ-08 & \textcolor{secondary}{PASS} \\
TC-I-LOAN-005 & GET userLoans vide             & I  & REQ-16 & \textcolor{secondary}{PASS} \\
TC-I-LOAN-006 & Limite 5 prêts                 & I  & REQ-13 & \textcolor{secondary}{PASS} \\
TC-I-LOAN-007 & GET loanCount                  & I  & REQ-16 & \textcolor{secondary}{PASS} \\
TC-I-LOAN-008 & Flux complet                   & I  & REQ-01,REQ-02,REQ-12 & \textcolor{secondary}{PASS} \\
\midrule
TC-S-001 & Parcours nouveau membre             & S  & REQ-01,REQ-02,REQ-12,REQ-14 & \textcolor{secondary}{PASS} \\
TC-S-002 & Enforcement limite 5               & S  & REQ-13 & \textcolor{secondary}{PASS} \\
TC-S-003 & Gestion genres flux                 & S  & REQ-09,REQ-10 & \textcolor{secondary}{PASS} \\
TC-S-004 & Concurrence même livre              & S  & REQ-15 & \textcolor{secondary}{PASS} \\
\midrule
TC-SE-AUTH-001 & Login valide                  & SE & REQ-02,REQ-18 & NE \\
TC-SE-AUTH-002 & Login email invalide          & SE & REQ-02,REQ-18 & NE \\
TC-SE-AUTH-003 & Login pwd incorrect (TC007)   & SE & REQ-02,REQ-18 & NE \\
TC-SE-AUTH-004 & Déconnexion                   & SE & REQ-02,REQ-18 & NE \\
TC-SE-AUTH-005 & Accès sans token              & SE & REQ-03,REQ-18 & NE \\
TC-SE-REG-001  & Inscription valide            & SE & REQ-01,REQ-18 & NE \\
TC-SE-REG-002  & Email déjà pris               & SE & REQ-01,REQ-18 & NE \\
TC-SE-REG-003  & Mot de passe faible          & SE & REQ-01,REQ-18 & NE \\
TC-SE-BOOK-001 & Affichage liste               & SE & REQ-04,REQ-18 & NE \\
TC-SE-BOOK-002 & Recherche titre               & SE & REQ-04,REQ-18 & NE \\
TC-SE-BOOK-003 & Ajout livre admin             & SE & REQ-05,REQ-18 & NE \\
TC-SE-BOOK-004 & Modification livre            & SE & REQ-06,REQ-18 & NE \\
TC-SE-BOOK-005 & Suppression livre             & SE & REQ-07,REQ-18 & NE \\
TC-SE-BOOK-006 & Aucun résultat                & SE & REQ-04,REQ-18 & NE \\
TC-SE-LOAN-001 & Affichage prêts               & SE & REQ-16,REQ-18 & NE \\
TC-SE-LOAN-002 & Emprunter livre               & SE & REQ-12,REQ-18 & NE \\
TC-SE-LOAN-003 & Retourner livre               & SE & REQ-14,REQ-18 & NE \\
TC-SE-LOAN-004 & Limite 5 prêts               & SE & REQ-13,REQ-18 & NE \\
TC-SE-LOAN-005 & Livres indisponibles          & SE & REQ-08,REQ-18 & NE \\
TC-SE-LOAN-006 & Flux emprunt complet          & SE & REQ-12,REQ-18 & NE \\

\bottomrule
\caption{Matrice cas de test $\times$ résultats}
\end{longtable}

% =============================================================================
\chapter{Analyse de Couverture}
% =============================================================================

\begin{tcolorbox}[colframe=primary, colback=rowalt!30,
                  title=Résumé de la couverture des exigences]
\begin{tabular}{lc}
  Exigences couvertes (≥ 1 cas de test) & 18 / 18 = \textbf{100\%} \\[4pt]
  Exigences totalement couvertes        & 15 / 18 = \textbf{83\%} \\[4pt]
  Exigences partiellement couvertes     & 3 / 18 = \textbf{17\%}
    (REQ-03, REQ-10, REQ-18) \\[4pt]
  Exigences non couvertes               & 0 / 18 = \textbf{0\%} \\
\end{tabular}
\end{tcolorbox}

\bigskip

\begin{tcolorbox}[colframe=secondary, colback=covered!40,
                  title=Résumé de l'exécution des cas de test]
\begin{tabular}{lc}
  Cas de test définis          & 86 \\[4pt]
  Exécutés (U + I + S)         & 63 \\[4pt]
  Non Exécutés (SE – Selenium) & 23 (nécessitent serveurs Angular + API démarrés) \\[4pt]
  PASS                         & 63 / 63 = \textbf{100\%} des tests exécutés \\[4pt]
  FAIL                         & 0 \\
\end{tabular}
\end{tcolorbox}

\section{Observations}

\begin{itemize}
  \item \textbf{REQ-03} (accès sans token) est partiellement couvert :
        le test Selenium TC-SE-AUTH-005 vérifie le comportement Angular,
        mais il n'y a pas de test d'intégration HTTP qui envoie explicitement
        une requête sans header \texttt{Authorization} vers un endpoint protégé.
  \item \textbf{REQ-10} (pagination genres) : les paramètres \texttt{page} et
        \texttt{size} aux valeurs limites sont testés en unitaire mais pas de
        manière intégrée.
  \item \textbf{REQ-18} (IHM Angular) : les 23 cas Selenium sont définis et
        prêts mais nécessitent un environnement avec
        \texttt{http://localhost:4200} et \texttt{http://localhost:5041} actifs.
\end{itemize}

\end{document}
```
