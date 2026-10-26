import os
from reportlab.lib.pagesizes import letter
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib import colors
from reportlab.platypus import SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle, HRFlowable, KeepTogether

pdf_path = r"c:\Users\genti\OneDrive\Desktop\Internship\Asset-Management-App\Prezantimi_Asset_Management_System.pdf"

doc = SimpleDocTemplate(
    pdf_path,
    pagesize=letter,
    rightMargin=45,
    leftMargin=45,
    topMargin=45,
    bottomMargin=45
)

styles = getSampleStyleSheet()

# Custom styles
title_style = ParagraphStyle(
    'DocTitle',
    parent=styles['Heading1'],
    fontName='Helvetica-Bold',
    fontSize=22,
    leading=26,
    textColor=colors.HexColor('#1E293B'),
    spaceAfter=6
)

subtitle_style = ParagraphStyle(
    'DocSubTitle',
    parent=styles['Normal'],
    fontName='Helvetica',
    fontSize=11,
    leading=15,
    textColor=colors.HexColor('#64748B'),
    spaceAfter=15
)

h1_style = ParagraphStyle(
    'SectionH1',
    parent=styles['Heading2'],
    fontName='Helvetica-Bold',
    fontSize=14,
    leading=18,
    textColor=colors.HexColor('#0F172A'),
    spaceBefore=14,
    spaceAfter=6
)

h2_style = ParagraphStyle(
    'SectionH2',
    parent=styles['Heading3'],
    fontName='Helvetica-Bold',
    fontSize=11,
    leading=15,
    textColor=colors.HexColor('#2563EB'),
    spaceBefore=8,
    spaceAfter=4
)

body_style = ParagraphStyle(
    'Body',
    parent=styles['Normal'],
    fontName='Helvetica',
    fontSize=10,
    leading=14,
    textColor=colors.HexColor('#334155'),
    spaceAfter=6
)

quote_style = ParagraphStyle(
    'SpokenScript',
    parent=styles['Normal'],
    fontName='Helvetica-Oblique',
    fontSize=9.5,
    leading=13.5,
    textColor=colors.HexColor('#1E293B'),
    leftIndent=12,
    rightIndent=12,
    spaceAfter=6
)

bullet_style = ParagraphStyle(
    'BulletText',
    parent=styles['Normal'],
    fontName='Helvetica',
    fontSize=9.5,
    leading=13.5,
    textColor=colors.HexColor('#334155'),
    leftIndent=15,
    spaceAfter=3
)

story = []

# Title & Metadata
story.append(Paragraph("Udhezuesi i Prezantimit: Asset Management System", title_style))
story.append(Paragraph("Permbledhje e Plote per Prezantimin e Internship-it | .NET 10 & Clean Architecture", subtitle_style))
story.append(HRFlowable(width="100%", thickness=1.5, color=colors.HexColor('#CBD5E1'), spaceAfter=12))

# Section: Pyetja e Swagger
story.append(Paragraph("1. Demonstrimi Live me Swagger UI (A eshte ide e mire?)", h1_style))
story.append(Paragraph(
    "<b>PO, absolutisht po!</b> Swagger UI eshte standardi profesional i industrise per backend API. "
    "Nuk ke nevoje per nje frontend te komplikuar per te treguar nje backend te shkelqyer. "
    "Swagger tregon qe kodi eshte funksional, i dokumentuar, dhe i testueshem live ne cast.", body_style))
story.append(Paragraph("<b>Si ta pergatisni Swagger-in 2 minuta para prezantimit:</b>", body_style))
story.append(Paragraph("&bull; Hapni shfletuesin ne <b>/swagger</b>.", bullet_style))
story.append(Paragraph("&bull; Thirrni <b>POST /api/auth/login</b> me kredencialet e Adminit ose IT-Managerit.", bullet_style))
story.append(Paragraph("&bull; Kopjoni token-in dhe klikoni butonin e gjelber <b>Authorize</b> lart ne Swagger (vendosni: Bearer &lt;token&gt;).", bullet_style))
story.append(Paragraph("&bull; Tani, gjate prezantimit, vetem klikoni <i>Try it out -> Execute</i> ne GET /api/assets ose Dashboard, dhe pergjigjja del live ne 1 sekonde!", bullet_style))
story.append(Spacer(1, 8))

# Section: Pjeset e Veqanta
story.append(Paragraph("2. Gjashte Pjeset e Veqanta te Projektit (The Senior Highlights)", h1_style))
story.append(Paragraph("Keto jane pikat arkitekturore qe e dallojne projektin tend nga 95% e projekteve te zakonshme te internship-it:", body_style))

features = [
    ("1. Dynamic Scoping & Global Query Filters (Siguri me Dizajn)",
     "IT-Manager sheh dhe menaxhon VETEM departamentin e vet. Kjo nuk eshte bere me IF-a manuale ne controllers, "
     "por automatikisht me <b>ScopeMiddleware</b> dhe <b>EF Core Global Query Filter</b>. Te gjitha queries mbi tabela "
     "filtrohen automatikisht ne nivel SQL."),

    ("2. EF Core 10 Audit Expression Interceptor (Audit pa humbur shpejtesi)",
     "Ne EF Core, metoda e shpejte <i>ExecuteUpdateAsync</i> anashkalon ChangeTracker-in. Me kete interceptor te avancuar "
     "te ndertuar me Expression Trees, fushat <i>UpdatedAt</i> dhe <i>UpdatedBy</i> shtohen gjithmone ne SQL query pa humbur performancen e bulk updates."),

    ("3. Background Processing me Hangfire (Qendrueshmeri ne SQL)",
     "Puna nuk behet ne memorie (e cila humb nese serveri ristartohet), por ruhet ne SQL Server. "
     "Cdo mengjes ne ora 08:00 kontrollohen garancite dhe ne ora 03:00 pastrohen tokens e skaduar automatikisht."),

    ("4. Hybrid Caching me Polly Circuit Breaker (Graceful Degradation)",
     "Dashboard-i perdor Redis per shpejtesi maksimale. Nese Redis bie, <b>Circuit Breaker</b> parandalon bllokimin e kerkesave "
     "dhe kalon pa gabime ne lexim direkt nga databaza pa e rrezuar aplikacionin."),

    ("5. Mbrojtja me Rate Limiting (ASP.NET Core Standard)",
     "Standardi me i ri i Microsoft me <i>FixedWindowRateLimiter</i>. Endpoint-et e ndjeshme si /login jane te kufizuara ne 5 kerkese/minute per IP "
     "per mbrojtje nga sulmet Brute-Force."),

    ("6. Inteligjenca Artificiale me Groq LLM (Security Anomaly Alert)",
     "Nese nje perdorues ben shume thirrje te shpejta, Hangfire nis nje job ne prapaskene ku modeli i AI analizon rruget e kerkuara "
     "dhe gjeneron nje tekst alarmi te personalizuar per administratorin.")
]

for title, desc in features:
    story.append(Paragraph(title, h2_style))
    story.append(Paragraph(desc, body_style))

story.append(Spacer(1, 8))

# Section: Fjalimi i Prezantimit
story.append(Paragraph("3. Fjalimi i Plote i Prezantimit (Fjale per Fjale)", h1_style))
story.append(Paragraph("Ky eshte fjalimi qe mund ta ndiqni gjate prezantimit. Eshte i qarte, logjik dhe i lehte per t'u mbajtur mend:", body_style))

speech_parts = [
    ("Pjesa 1: Hyrja & Problemi i Biznesit (1 min)",
     "\"Pershendetje te gjitheve. Sot po ju prezantoj projektin tim te internship-it: Asset Management System (AMS).<br/><br/>"
     "Ne cdo kompani, menaxhimi i pajisjeve te IT-se (laptopa, monitore, servera) behet shpejt nje kaos nese mbahet neper tabela Excel-i: "
     "nuk dihet me siguri kush cfare pajisje ka ne dorezim, cilave po u skadon garancia, apo ne cilin departament ndodhen.<br/><br/>"
     "Qellimi im ka qene te ndertoj nje sistem qendror, modern dhe te automatizuar me .NET 10 dhe Clean Architecture qe heq punen manuale "
     "dhe i jep kompanise kontroll te plote mbi asetet e saj fizike.\""),

    ("Pjesa 2: Rolet & Ndarja e Qasjes (1 min)",
     "\"Sistemi bazohet ne tre nivele rolesh:<br/>"
     "&bull; <b>Admini:</b> Ka kontroll global mbi gjithe sistemin, menaxhon perdoruesit dhe transferimet organizative.<br/>"
     "&bull; <b>IT-Manageri:</b> Sheh dhe menaxhon VETEM asetet e departamentit te vet. Kjo eshte e izoluar ne menyre te hekurt ne kod permes Global Query Filters.<br/>"
     "&bull; <b>Punonjesi (Employee):</b> Sheh vetem pajisjet qe i jane caktuar personalisht.<br/><br/>"
     "Per siguri kam perdorur JWT Access Tokens (15 min) me Refresh Tokens qe rrotullohen automatikisht, si dhe Rate Limiting te bazuar ne IP per te parandaluar sulmet me fjalekalime.\""),

    ("Pjesa 3: Cikli i Jetes se Pajisjeve (1.5 min)",
     "\"Cdo aset kalon neper nje cikel te plote jetesor:<br/>"
     "&bull; Regjistrohet me tag unik, numer serik, cmim dhe date garancie.<br/>"
     "&bull; I caktohet nje punonjesi (statusi kalon ne InUse), ose kthehet ne magazine (InStock) kur punonjesi largohet.<br/>"
     "&bull; Kalon ne riparim (UnderMaintenance) ose ckycet nga perdorimi (Retired).<br/>"
     "&bull; Per menaxhmentin, kemi Dashboard-in Analitik qe llogarit vleren totale financiare te aseteve, ndarjen sipas kategorive dhe moshen e pajisjeve.\""),

    ("Pjesa 4: Pjesa e Avancuar & Prapaskena (2 min)",
     "\"Ajo qe dua te theksoj me shume eshte prapaskena inxhinierike e ketij sistemi:<br/>"
     "&bull; <b>Hangfire Jobs:</b> Sistemi punon vete pa nderhyre njeriu. Cdo mengjes ne ora 08:00 kontrollon garancite dhe lajmeron per pajisjet qe po skadojne, kurse naten pastron sesionet e vjetra.<br/>"
     "&bull; <b>Shpejtesia me Redis & Circuit Breaker:</b> Dashboard-i lexohet nga memoria e shpejte. Nese Redis bie, aplikacioni nuk rrezohet por vazhdon normalisht nga databaza.<br/>"
     "&bull; <b>AI Security Anomaly:</b> Nese verehen shume thirrje te shpejta, nje model inteligjence artificiale (Groq LLM) analizon trafikun dhe pergatit nje raport sigurie per administratorin.\""),

    ("Pjesa 5: Demonstrimi Live & Mbyllja (1 min)",
     "\"Tani po ju tregoj sistemin live ne Swagger:<br/>"
     "&bull; Po bej login si Admin per te marre token-in.<br/>"
     "&bull; Po therras listen e aseteve dhe po bej caktimin e nje laptopi per nje punonjes.<br/>"
     "&bull; Dhe ketu ne /hangfire mund te shihni dashboard-in me punet e skeduluara qe vrapojne ne prapaskene.<br/><br/>"
     "Ju faleminderit shume per vemendjen dhe jam i gatshem per cdo pyetje!\"")
]

for p_title, p_speech in speech_parts:
    story.append(Paragraph(p_title, h2_style))
    story.append(Paragraph(p_speech, quote_style))
    story.append(Spacer(1, 4))

story.append(Spacer(1, 8))

# Section: Pyetjet e Pritshme
story.append(Paragraph("4. Pergjigjet e Shpejta per Pyetjet e Mentorise", h1_style))
faq = [
    ("Pse perdore ErrorOr ne vend te Exceptions?",
     "Exceptions kane kosto te larte ne .NET (stack-trace). Gabimet si 'Fjalekalimi gabim' ose 'Aseti nuk u gjet' nuk jane gabime sistemi, por rrjedhe normale e biznesit. ErrorOr e ben kodin me te shpejte dhe me te paster."),

    ("Pse Hangfire e jo BackgroundService te thjeshte?",
     "BackgroundService jeton vetem ne memorie: po u fik serveri ose po beri restart, oraret dhe punet humbasin. Hangfire i ruan te gjitha ne SQL Server, pra eshte 100% durable."),

    ("Ku i ke testet e automatizuara?",
     "Pasi koha e internship-it ishte e fokusuar ne arkitekture, logjike dhe resilience, testet manuale jane kryer me Postman dhe Swagger. Por meqe projekti eshte Clean Architecture me Repository Pattern, shtimi i xUnit dhe Moq eshte hapi i pare i radhes.")
]

for q, a in faq:
    story.append(Paragraph("<b>Q: " + q + "</b>", h2_style))
    story.append(Paragraph("<b>A:</b> " + a, body_style))

doc.build(story)
print("PDF GENERATED SUCCESSFULLY AT:", pdf_path)
