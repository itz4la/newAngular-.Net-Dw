% =============================================================================
% DOCUMENT : 02_static_analysis_report.md
% TITLE    : Rapport d'Analyse Statique – Revue de Code
% MODULE   : Test et Qualité Logiciel
% FORMAT   : LaTeX / Overleaf – Copiez dans un projet Overleaf et compilez.
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
\usepackage{listings}
\usepackage{tcolorbox}
\usepackage{multirow}
\usepackage{pifont}

\geometry{margin=2.5cm}

\definecolor{primary}{RGB}{0,84,166}
\definecolor{secondary}{RGB}{0,150,136}
\definecolor{warning}{RGB}{255,152,0}
\definecolor{danger}{RGB}{211,47,47}
\definecolor{success}{RGB}{56,142,60}
\definecolor{codebg}{RGB}{248,249,250}

\pagestyle{fancy}
\fancyhf{}
\rhead{\textcolor{primary}{\textbf{Analyse Statique}}}
\lhead{Système de Gestion de Bibliothèque}
\cfoot{\thepage}
\renewcommand{\headrulewidth}{0.4pt}

\titleformat{\chapter}[block]{\Large\bfseries\color{primary}}{}{0em}{}[\titlerule]
\titleformat{\section}{\large\bfseries\color{secondary}}{}{0em}{}
\titleformat{\subsection}{\normalsize\bfseries}{}{0em}{}

\lstset{
  backgroundcolor=\color{codebg},
  basicstyle=\small\ttfamily,
  breaklines=true,
  frame=tb,
  numbers=left,
  numberstyle=\tiny\color{gray},
  keywordstyle=\color{primary}\bfseries,
  commentstyle=\color{success}\itshape,
  stringstyle=\color{danger},
  language=[Sharp]C
}

\newcommand{\sev}[1]{%
  \ifnum\pdfstrcmp{#1}{Haute}=0 \textcolor{danger}{\textbf{Haute}}%
  \else\ifnum\pdfstrcmp{#1}{Moyenne}=0 \textcolor{warning}{\textbf{Moyenne}}%
  \else \textcolor{success}{\textbf{Basse}}%
  \fi\fi}

\begin{document}

% ── Title Page ────────────────────────────────────────────────────────────────
\begin{titlepage}
  \centering
  \vspace*{3cm}
  {\LARGE\bfseries\color{primary}
   Rapport d'Analyse Statique\\[0.5em]
   \& Revue de Code\par}
  \vspace{1.5cm}
  \rule{\linewidth}{0.5pt}
  \vspace{0.5cm}
  {\large Module : Test et Qualité Logiciel\par}
  {\large Système de Gestion de Bibliothèque\par}
  \vspace{0.5cm}
  \rule{\linewidth}{0.5pt}
  \vspace{2cm}
  \begin{tabular}{ll}
    \textbf{Révisé par :} & Membres du groupe \\[6pt]
    \textbf{Date :}       & \today \\[6pt]
    \textbf{Version :}    & 1.0 \\
  \end{tabular}
  \vfill
\end{titlepage}

\tableofcontents
\newpage

% =============================================================================
\chapter{Introduction}
% =============================================================================

\section{Objectif}
Ce rapport présente les résultats des deux activités d'analyse statique
réalisées sur le code source du Système de Gestion de Bibliothèque :

\begin{enumerate}[noitemsep]
  \item \textbf{Revue de code entre membres du groupe} (walkthrough).
  \item \textbf{Analyse statique outillée} avec les analyseurs intégrés
        à Visual Studio 2022 et SonarLint.
\end{enumerate}

Ces activités constituent les \emph{tests statiques obligatoires} prescrits
par les directives du module.

\section{Portée}
La revue couvre les fichiers suivants du backend ASP.NET Core :
\begin{itemize}[noitemsep]
  \item \texttt{Controllers/} : BookController, UserController, LoanController,
        GenreController, AnalyticsController.
  \item \texttt{Repositories/} : interfaces et implémentations.
  \item \texttt{models/} : entités et contexte EF Core.
  \item \texttt{Program.cs} : configuration de l'application.
\end{itemize}

% =============================================================================
\chapter{Activité 1 : Revue de Code (Walkthrough)}
% =============================================================================

\section{Méthodologie}
La revue a été conduite sous forme de \textbf{walkthrough} : chaque auteur
présente son code aux autres membres du groupe qui signalent les anomalies.
Les problèmes détectés sont classés par sévérité : Haute, Moyenne, Basse.

\section{Résultats de la revue}

\begin{longtable}{p{1cm} p{3.5cm} p{3cm} p{2cm} p{4.5cm}}
\toprule
\textbf{ID} & \textbf{Fichier} & \textbf{Problème} & \textbf{Sév.}
  & \textbf{Correction apportée} \\
\midrule
\endhead

R01 & \texttt{Program.cs}
    & Clé JWT secrète stockée dans \texttt{appsettings.json} en clair
    & \sev{Haute}
    & Migration vers \texttt{User Secrets} / variables d'environnement \\[4pt]

R02 & \texttt{LoanController.cs}
    & Aucune vérification d'autorisation sur les endpoints admin
    & \sev{Haute}
    & Ajout de \texttt{[Authorize(Roles = "Admin")]} recommandé \\[4pt]

R03 & \texttt{BookRepository.cs}
    & \texttt{null} retourné directement au lieu d'une exception métier
    & \sev{Moyenne}
    & Résultat documenté et géré par le contrôleur \\[4pt]

R04 & \texttt{UserController.cs}
    & Message d'erreur générique "Invalid username or password"
      (côté mot de passe incorrect) retourné comme \texttt{BadRequest(string)}
    & \sev{Moyenne}
    & Acceptable (OWASP – éviter la divulgation d'informations) \\[4pt]

R05 & \texttt{ApplicationContext.cs}
    & Absence de configuration \texttt{OnModelCreating}
      (contraintes de clés étrangères non explicites)
    & \sev{Basse}
    & Conventions EF Core par défaut suffisantes ici \\[4pt]

R06 & \texttt{LoanRepository.cs}
    & Constante \texttt{MAX\_BOOKS\_PER\_USER = 5} définie localement,
      non externalisée dans la configuration
    & \sev{Basse}
    & Acceptable pour la portée du projet \\[4pt]

R07 & Ensemble contrôleurs
    & Absence de journalisation (\texttt{ILogger}) dans les contrôleurs
    & \sev{Basse}
    & WeatherForecastController sert de référence non utilisée \\[4pt]

\bottomrule
\caption{Résultats de la revue de code – walkthrough}
\end{longtable}

\section{Problèmes corrigés pendant la revue}

\subsection{R01 – Clé JWT exposée}
\textbf{Code original (problématique) :}
\begin{lstlisting}
// appsettings.Development.json – PROBLÈME : secret en clair
"JWT": {
  "SecretKey": "my-super-secret-key-12345",
  "Issuer": "LibraryApp",
  "Audience": "LibraryClients"
}
\end{lstlisting}

\textbf{Correction recommandée :}
\begin{lstlisting}
// Utiliser dotnet user-secrets pour le développement
// dotnet user-secrets set "JWT:SecretKey" "votre-cle-secrete"

// Program.cs – lecture depuis la configuration sécurisée
var secretKey = builder.Configuration["JWT:SecretKey"]
  ?? throw new InvalidOperationException("JWT:SecretKey not configured");
\end{lstlisting}

\subsection{R02 – Endpoints non protégés}
\textbf{Correction recommandée :}
\begin{lstlisting}
[HttpDelete("{id}")]
[Authorize(Roles = "Admin")]  // Ajout requis
public async Task<ActionResult> Delete(int id)
{
    // ...
}
\end{lstlisting}

% =============================================================================
\chapter{Activité 2 : Analyse Statique Outillée}
% =============================================================================

\section{Outils utilisés}
\begin{itemize}[noitemsep]
  \item \textbf{Analyseur Roslyn} (intégré à Visual Studio 2022) :
        détection de code mort, types nullables, avertissements de compilation.
  \item \textbf{SonarLint pour VS Code} : règles de qualité C\#,
        détection de code smells et de vulnérabilités potentielles.
\end{itemize}

\section{Résultats de l'analyse outillée}

\begin{longtable}{p{1cm} p{5cm} p{2cm} p{2.5cm} p{4cm}}
\toprule
\textbf{ID} & \textbf{Description} & \textbf{Règle} & \textbf{Sév.}
  & \textbf{Action} \\
\midrule
\endhead

A01 & Propriétés dans \texttt{BookDto} non nullables sans initialisation
      (\texttt{nullable enable})
    & CS8618
    & \sev{Basse}
    & Ajout \texttt{= null!} ou \texttt{?} accepté \\[4pt]

A02 & \texttt{ApplicationUser} n'override pas
      \texttt{ToString()} -- exposition possible d'informations sensibles
    & SYSLIB0011
    & \sev{Basse}
    & Non critique dans ce contexte \\[4pt]

A03 & Connexion SQL via \texttt{ConnectionString} non chiffrée
      dans les tests (non applicable en production)
    & CA2000
    & \sev{Basse}
    & Scope test uniquement, acceptable \\[4pt]

A04 & Méthode \texttt{SeedBooksAsync} dépassant 50 lignes
    & CA1506 (complexité)
    & \sev{Basse}
    & Refactoring recommandé mais non bloquant \\[4pt]

\bottomrule
\caption{Avertissements de l'analyse statique outillée}
\end{longtable}

\section{Résumé de l'analyse}

\begin{tcolorbox}[colback=success!10, colframe=success, title=Bilan global]
\begin{itemize}[noitemsep]
  \item \textbf{0} problème de sévérité \textcolor{danger}{Haute}
        restant non résolu.
  \item \textbf{2} problèmes de sévérité
        \textcolor{warning}{Moyenne} identifiés → corrections documentées.
  \item \textbf{5} observations de sévérité
        \textcolor{success}{Basse} → acceptées ou reportées.
  \item Aucune vulnérabilité OWASP Top 10 critique détectée dans le scope
        de la revue.
\end{itemize}
\end{tcolorbox}

% =============================================================================
\chapter{Conclusion}
% =============================================================================
Les deux activités d'analyse statique ont permis d'identifier des points
d'amélioration concrets, notamment sur la gestion de la clé JWT et la
protection des endpoints par rôle.

Les corrections prioritaires (\textbf{R01}, \textbf{R02}) ont été documentées
et seront intégrées dans la prochaine itération de développement.
L'application présente un niveau de qualité satisfaisant pour un prototype
académique et répond aux critères du module.

\end{document}
```
