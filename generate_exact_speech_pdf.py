from reportlab.lib.pagesizes import letter
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib import colors
from reportlab.platypus import SimpleDocTemplate, Paragraph, Spacer, HRFlowable

pdf_path = r"c:\Users\genti\OneDrive\Desktop\Internship\Asset-Management-App\Prezantimi_Fjalimi.pdf"

doc = SimpleDocTemplate(
    pdf_path,
    pagesize=letter,
    rightMargin=45,
    leftMargin=45,
    topMargin=40,
    bottomMargin=40
)

styles = getSampleStyleSheet()

title_style = ParagraphStyle(
    'DocTitle',
    parent=styles['Heading1'],
    fontName='Helvetica-Bold',
    fontSize=20,
    leading=24,
    textColor=colors.HexColor('#0F172A'),
    spaceAfter=4
)

subtitle_style = ParagraphStyle(
    'DocSubTitle',
    parent=styles['Normal'],
    fontName='Helvetica',
    fontSize=10,
    leading=14,
    textColor=colors.HexColor('#475569'),
    spaceAfter=10
)

part_title_style = ParagraphStyle(
    'PartTitle',
    parent=styles['Heading2'],
    fontName='Helvetica-Bold',
    fontSize=12.5,
    leading=16.5,
    textColor=colors.HexColor('#1D4ED8'),
    spaceBefore=10,
    spaceAfter=5
)

stage_dir_style = ParagraphStyle(
    'StageDirection',
    parent=styles['Normal'],
    fontName='Helvetica-Oblique',
    fontSize=9,
    leading=13,
    textColor=colors.HexColor('#D97706'),
    spaceAfter=4
)

speech_style = ParagraphStyle(
    'Speech',
    parent=styles['Normal'],
    fontName='Helvetica',
    fontSize=10,
    leading=14.5,
    textColor=colors.HexColor('#1E293B'),
    spaceAfter=6
)

feature_title_style = ParagraphStyle(
    'FeatureTitle',
    parent=styles['Heading3'],
    fontName='Helvetica-Bold',
    fontSize=10.5,
    leading=14.5,
    textColor=colors.HexColor('#0F172A'),
    spaceBefore=5,
    spaceAfter=2
)

feature_desc_style = ParagraphStyle(
    'FeatureDesc',
    parent=styles['Normal'],
    fontName='Helvetica',
    fontSize=9.5,
    leading=13.5,
    textColor=colors.HexColor('#334155'),
    spaceAfter=5
)

tip_title_style = ParagraphStyle(
    'TipTitle',
    parent=styles['Heading3'],
    fontName='Helvetica-Bold',
    fontSize=11,
    leading=15,
    textColor=colors.HexColor('#047857'),
    spaceBefore=10,
    spaceAfter=4
)

tip_text_style = ParagraphStyle(
    'TipText',
    parent=styles['Normal'],
    fontName='Helvetica',
    fontSize=9.5,
    leading=13.5,
    textColor=colors.HexColor('#334155'),
    spaceAfter=3
)

story = []

# Title
story.append(Paragraph("Prezantimi i Plote: Asset Management System (AMS)", title_style))
story.append(Paragraph("Fjalimi i strukturuar per prezantimin e internship-it | Kohezgjatja: 5 - 7 minuta", subtitle_style))
story.append(HRFlowable(width="100%", thickness=1.5, color=colors.HexColor('#2563EB'), spaceAfter=8))

# Pjesa 1
story.append(Paragraph("Pjesa 1: Hyrja dhe Problemi (Cfare zgjidh ky projekt?) &mdash; 1 minute", part_title_style))
p1 = (
    "\"Pershendetje te gjitheve. Sot po ju prezantoj projektin tim te internship-it: <b>Asset Management System (AMS)</b>.<br/><br/>"
    "Ne cdo kompani qe rritet, nje nga sfidat me te medha te departamentit te IT-se eshte menaxhimi i pajisjeve: kush cfare laptopi ka, "
    "a ka garanci ende ajo pajisje, ne cilin departament ndodhet, apo kur duhet te zevendesohet nje server.<br/><br/>"
    "Qellimi im ka qene te ndertoj nje sistem te sigurt, te shpejte dhe te automatizuar qe heq punen manuale me tabela Excel-i "
    "dhe i jep kompanise kontroll te plote mbi te gjitha asetet e saj fizike.\""
)
story.append(Paragraph(p1, speech_style))

# Pjesa 2
story.append(Paragraph("Pjesa 2: Rolet dhe Siguria (Kush ka qasje ne cfare?) &mdash; 1 minute", part_title_style))
p2 = (
    "\"Sistemi bazohet ne tre nivele qasjeje, duke garantuar qe secili sheh vetem ate qe i takon:<br/><br/>"
    "&bull; <b>Admini:</b> Ka kontroll global mbi gjithe kompanine &mdash; menaxhon perdoruesit, krijon departamente dhe transferon punonjesit apo asetet mes departamenteve.<br/>"
    "&bull; <b>IT-Manageri:</b> Menaxhon asetet dhe stafin <b>vetem brenda departamentit te tij</b>. Kete e kam siguruar automatikisht ne nivel kodi: nje menaxher i IT-se nuk mund te shohe apo modifikoje asetet e nje departamenti tjeter.<br/>"
    "&bull; <b>Employee (Punonjesi):</b> Mund te shohe vetem pajisjet qe i jane besuar personalisht.<br/><br/>"
    "Per sigurine kam perdorur <b>JWT Tokens</b> me jetegjatesi 15 minuta dhe <b>Refresh Tokens</b> qe rrotullohen automatikisht, "
    "si dhe <b>Rate Limiting</b> per te mbrojtur sistemin nga sulmet me fjalekalime apo ngarkesat e panevojshme.\""
)
story.append(Paragraph(p2, speech_style))

# Pjesa 3
story.append(Paragraph("Pjesa 3: Cikli i Jetes se nje Pajisjeje (Funksionaliteti Kryesor) &mdash; 1.5 minuta", part_title_style))
story.append(Paragraph("<i>(Ketu mund te tregosh shkurtimisht me dore ose ne ekran te Swagger lista e aseteve)</i>", stage_dir_style))
p3 = (
    "\"Cdo aset ne sistem kalon neper nje cikel te plote jetesor:<br/><br/>"
    "&bull; Regjistrohet me barkod/tag unik, numer serik, cmim blerjeje dhe date skadimi te garancise.<br/>"
    "&bull; Mund t'i caktohet nje punonjesi kur fillon punen (<b>InUse</b>), ose te kthehet perseri ne stok (<b>InStock</b>) kur ai largohet.<br/>"
    "&bull; Pajisjet mund te kalojne ne mirembajtje (<b>UnderMaintenance</b>) ose te nxirren nga perdorimi (<b>Retired</b>).<br/>"
    "&bull; Per menaxhmentin, kemi nje <b>Dashboard Analitik</b> qe tregon ne kohe reale vleren totale financiare te pajisjeve, shperndarjen sipas departamenteve dhe vjetersine e tyre.\""
)
story.append(Paragraph(p3, speech_style))

# Pjesa 4 (Zëvendësuar me 6 Pjesët e Veçanta / Killer Features të zgjeruara)
story.append(Paragraph("Pjesa 4: Te gjitha Pjeset e Veqanta (The \"Killer\" Features) te zgjeruara &mdash; 2 minuta", part_title_style))
story.append(Paragraph("<i>Keto jane 6 pikat kryesore te prapaskenes qe e bejne kete projekt te nivelit te larte:</i>", stage_dir_style))

features = [
    ("1. Dynamic Scoping & Global Query Filters (Multi-tenancy brenda departamenteve):",
     "<b>Pse eshte e veçante:</b> IT-Manageri sheh vetem asetet e departamentit te vet. Shumica e fillestareve e bejne kete me if/else ne cdo controller. Ne kete projekt eshte bere me <b>ScopeMiddleware</b> dhe <b>EF Core Global Query Filter</b> &mdash; filtri zbatohet automatikisht ne nivel SQL pa pasur nevoje te shkruash Where(...) me dore."),

    ("2. EF Core 10 Audit Expression Interceptor (AuditExecuteUpdateInterceptor.cs):",
     "<b>Pse eshte e veçante:</b> Metoda ExecuteUpdateAsync e EF Core eshte jashtezakonisht e shpejte per ndryshime ne mase (bulk updates), por e ka nje te mete: anashkalon ChangeTracker-in, pra humbet auditimi (UpdatedAt). Ketu eshte ndertuar nje <b>Expression Interceptor</b> qe e modifikon vete pemen e SQL-it qe UpdatedAt dhe UpdatedBy te vendosen gjithmone automatikisht."),

    ("3. Background Jobs me Hangfire (Qendrueshmeri ne SQL Server):",
     "<b>Pse eshte e veçante:</b> Ne vend qe te perdoret nje BackgroundService i thjeshte qe fshihet po u fik kompjuteri, Hangfire ruan gjithcka ne SQL Server. Skedulimi i ores 08:00 (per garancite) dhe ora 03:00 (per pastrimin e sesioneve) nuk humbasin kurre."),

    ("4. Resilience me Polly Circuit Breaker te Redis (CacheService.cs):",
     "<b>Pse eshte e veçante:</b> Nese Redis rrezohet, aplikacionet e thjeshta bllokohen me 500 Error. Ne kete aplikacion, <b>Circuit Breaker</b> pas 3 deshtimesh e hap qarkun per 10 sekonda dhe lexon direkt nga databaza pa e verejtur perdoruesi fare deshtimin e cache-it."),

    ("5. Mbrojtja me Rate Limiting (Standardi i ASP.NET Core):",
     "<b>Pse eshte e veçante:</b> Endpoint-et e ndjeshme si /login dhe /forgot-password mbrohen me <b>FixedWindowRateLimiter</b> (5 kerkesa ne minute per IP) per te bllokuar sulmet Brute-Force."),

    ("6. Inteligjenca Artificiale me Groq LLM (Security Anomaly Detection):",
     "<b>Pse eshte e veçante:</b> Nese nje perdorues ben kerkesa te shpeshta anormale, Hangfire therret modelin e inteligjences artificiale (Groq) ne sfond per te gjeneruar nje raport tekstual te kuptueshem per stafin e sigurise.")
]

for f_title, f_desc in features:
    story.append(Paragraph(f_title, feature_title_style))
    story.append(Paragraph(f_desc, feature_desc_style))

# Pjesa 5
story.append(Paragraph("Pjesa 5: Demonstrimi Live dhe Permbyllja &mdash; 1 minute", part_title_style))
p5 = (
    "\"Tani po jua tregoj shkurtimisht sistemin ne veprim ne Swagger:<br/><br/>"
    "&bull; <i>(Ben Login si Admin dhe merr token-in per ta vendosur te butoni Authorize)</i>.<br/>"
    "&bull; <i>(Therret GET /api/assets per te treguar listen dhe ben caktimin e nje aseti te punonjesi)</i>.<br/>"
    "&bull; <i>(Hap shpejt /hangfire per te treguar punet automatike ne sfond)</i>.<br/><br/>"
    "Ju faleminderit per vemendjen dhe jam i hapur per cdo pyetje apo sugjerim nga ana juaj!\""
)
story.append(Paragraph(p5, speech_style))

story.append(HRFlowable(width="100%", thickness=1, color=colors.HexColor('#E2E8F0'), spaceAfter=6))

# Këshillat
story.append(Paragraph("Keshilla te arta per prezantimin:", tip_title_style))
story.append(Paragraph("<b>1. Mos fol shpejt:</b> Merr fryme pas cdo fjalie. Pauzat e shkurtra te bejne te dukesh shume i sigurt ne vete.", tip_text_style))
story.append(Paragraph("<b>2. Nese nuk di dicka te thelle:</b> Mos sajo. Thuaj: <i>\"Te kjo pjese jam bazuar ne kete zgjidhje per shkak te tolerances ndaj gabimeve, por do te doja shume te degjoja mendimin tuaj se si e keni trajtuar kete ne projektet e kompanise.\"</i>", tip_text_style))
story.append(Paragraph("<b>3. Mbaje kete leter prane:</b> Mund ta kesh kete flete te printuar ose ne telefon para vetes per t'u orientuar natyrshem pike pas pike.", tip_text_style))

doc.build(story)
print("SUCCESSFULLY REBUILT:", pdf_path)
