<?xml version="1.0" encoding="utf-8"?>
<!--
    GCModeller :: VirtualCell (GCMarkup 3.0) visual report stylesheet

    Usage (offline, recommended):
        run XslReport.exe in report mode with the model, xsl, out and limit options.

    Usage (browser XSLT 1.0):
        add an xml-stylesheet processing instruction pointing at this file
        after the XML declaration of the model document.

    Notes:
      * XSLT 1.0 only. No EXSLT, no extension functions, no xsl:for-each-group.
      * Every model element lives in the default namespace
        https://bioCAD.gcmodeller.org/XML/schema_revision/GCMarkup_3.0
        so it MUST be addressed with the gcm: prefix in XSLT 1.0.
      * Attributes are unprefixed and case sensitive (@ID, @classId, @protein_id ...).
      * The report is a single self-contained HTML file: inline CSS, inline JS, inline SVG,
        no external fonts, scripts or images.
      * Large models are truncated: each detail panel renders at most $limit rows and
        prints the exact total of that section. Default limit = 50.
      * CSS/JS deliberately contain no "<", ">" or "&" characters so that the stylesheet
        works with both browser XSLT engines and .NET XslCompiledTransform writing to an
        XmlWriter (disable-output-escaping is intentionally NOT used).
      * Non void HTML elements (div, span, i, td, th) always carry a text node: the xml
        output method serializes empty elements as self closing tags, which an HTML5
        parser treats as unclosed start tags and would swallow all following markup.
-->
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:gcm="https://bioCAD.gcmodeller.org/XML/schema_revision/GCMarkup_3.0"
                xmlns:vck="https://bioCAD.gcmodeller.org/vcellkit/"
                xmlns:gcmod="https://gcmodeller.org"
                exclude-result-prefixes="gcm vck gcmod">

  <!-- method=xml keeps SVG camelCase attributes (viewBox, preserveAspectRatio) intact.
       doctype-system emits a valid HTML5 legacy-compat doctype without DOE. -->
  <xsl:output method="xml" encoding="utf-8" omit-xml-declaration="yes" indent="no"
              doctype-system="about:legacy-compat"/>
  <xsl:strip-space elements="*"/>

  <xsl:param name="limit" select="50"/>
  <xsl:param name="reportTitle" select="'GCModeller Virtual Cell Report'"/>
  <xsl:param name="generatedAt" select="''"/>

  <!-- Muenchian grouping indexes (XSLT 1.0) -->
  <xsl:key name="k-compound-class" match="gcm:compounds/gcm:compound" use="string(@classId)"/>
  <xsl:key name="k-publication-year" match="gcm:evidence/gcm:publication" use="string(@year)"/>
  <xsl:key name="k-regulation-family" match="gcm:regulations/gcm:transcription" use="string(gcm:motif/@family)"/>

  <!-- ================================================================== -->
  <!-- Global statistics. Evaluated once, reused by every section.          -->
  <!-- ================================================================== -->
  <xsl:variable name="vc" select="/gcm:VirtualCell"/>
  <xsl:variable name="genome" select="$vc/gcm:genome"/>
  <xsl:variable name="meta" select="$vc/gcm:metabolome"/>
  <xsl:variable name="genes" select="$genome/gcm:replicon/gcm:operons/gcm:TranscriptUnit/gcm:genes"/>
  <xsl:variable name="replicons" select="$genome/gcm:replicon"/>
  <xsl:variable name="tus" select="$genome/gcm:replicon/gcm:operons/gcm:TranscriptUnit"/>
  <xsl:variable name="regulations" select="$genome/gcm:regulations/gcm:transcription"/>
  <xsl:variable name="proteins" select="$genome/gcm:proteins/gcm:protein"/>
  <xsl:variable name="regulons" select="$genome/gcm:regulons/gcm:regulon"/>
  <xsl:variable name="bindingSites" select="$genome/gcm:bindingSites/gcm:bindingSite"/>
  <xsl:variable name="promoters" select="$genome/gcm:promoters/gcm:promoter"/>
  <xsl:variable name="terminators" select="$genome/gcm:terminators/gcm:terminator"/>
  <xsl:variable name="proteinFeatures" select="$genome/gcm:proteinFeatures/gcm:proteinFeature"/>
  <xsl:variable name="rnas" select="$genome/gcm:replicon/gcm:RNAs/gcm:RNA"/>
  <xsl:variable name="compounds" select="$meta/gcm:compounds/gcm:compound"/>
  <xsl:variable name="reactionNodes" select="$meta/gcm:reactions/gcm:enzymatic/gcm:reaction | $meta/gcm:reactions/gcm:none_enzymatic/gcm:reaction"/>
  <xsl:variable name="enzymaticReactions" select="$meta/gcm:reactions/gcm:enzymatic/gcm:reaction"/>
  <xsl:variable name="nonEnzymaticReactions" select="$meta/gcm:reactions/gcm:none_enzymatic/gcm:reaction"/>
  <xsl:variable name="transportations" select="$meta/gcm:reactions/gcm:transportation/gcm:Transportation"/>
  <xsl:variable name="enzymes" select="$meta/gcm:enzymes/gcm:enzyme"/>
  <xsl:variable name="pathways" select="$meta/gcm:pathwayMaps/gcm:FunctionalCategory/gcm:pathway"/>
  <xsl:variable name="categories" select="$meta/gcm:pathwayMaps/gcm:FunctionalCategory"/>
  <xsl:variable name="compartments" select="$vc/gcm:compartments/gcm:compartment"/>
  <xsl:variable name="publications" select="$vc/gcm:evidence/gcm:publication"/>
  <xsl:variable name="classTerms" select="$vc/gcm:classTerms/gcm:classTerm"/>
  <xsl:variable name="reactionTotal" select="count($reactionNodes)"/>

  <!-- ================================================================== -->
  <!-- Root template                                                       -->
  <!-- ================================================================== -->
  <xsl:template match="/">
    <html lang="zh-CN">
      <head>
        <meta charset="utf-8"/>
        <meta name="viewport" content="width=device-width, initial-scale=1"/>
        <meta name="generator" content="GCModeller VirtualCell.xsl"/>
        <link rel="icon" href="data:,"/>
        <title><xsl:value-of select="$reportTitle"/></title>
        <style>
          <xsl:text>
:root{--bg:#0B1220;--bg2:#111C2E;--bg3:#1B2A41;--txt:#E6EDF3;--muted:#94A3B8;--teal:#14B8A6;--blue:#2563EB;--violet:#7C3AED;--green:#22C55E;--amber:#F59E0B;--red:#EF4444;--sky:#38BDF8;--line:rgba(148,163,184,.16);--glass:rgba(27,42,65,.62)}
*{box-sizing:border-box}
html{scroll-behavior:smooth}
body{margin:0;background:radial-gradient(1200px 700px at 12% -10%,rgba(37,99,235,.20),transparent 62%),radial-gradient(980px 620px at 92% 0%,rgba(20,184,166,.16),transparent 62%),radial-gradient(800px 800px at 50% 110%,rgba(124,58,237,.12),transparent 60%),var(--bg);color:var(--txt);font-family:'Noto Sans','Segoe UI',system-ui,-apple-system,BlinkMacSystemFont,sans-serif;font-size:14px;line-height:1.6}
a{color:var(--sky)}
header.top{position:sticky;top:0;z-index:30;backdrop-filter:blur(18px);background:rgba(11,18,32,.80);border-bottom:1px solid var(--line)}
.top-in{max-width:1480px;margin:auto;padding:13px 28px;display:flex;align-items:center;gap:18px;flex-wrap:wrap}
.brand{display:flex;align-items:center;gap:12px;min-width:250px}
.logo{width:38px;height:38px;border-radius:12px;background:linear-gradient(135deg,var(--teal),var(--blue) 55%,var(--violet));box-shadow:0 0 24px rgba(20,184,166,.38);display:grid;place-items:center;font-weight:800;color:#04121a;font-size:16px}
.brand h1{font-size:17px;margin:0;letter-spacing:.3px}
.brand p{margin:0;color:var(--muted);font-size:11px}
nav.anchors{display:flex;gap:8px;margin-left:auto;flex-wrap:wrap;align-items:center}
nav.anchors a{color:var(--muted);text-decoration:none;font-size:12.5px;padding:7px 14px;border:1px solid var(--line);border-radius:999px;transition:.25s}
nav.anchors a:hover{color:var(--txt);border-color:var(--teal);background:rgba(20,184,166,.10);transform:translateY(-1px)}
nav.anchors button{cursor:pointer;color:var(--muted);font-size:12.5px;padding:7px 14px;border:1px solid var(--line);border-radius:999px;background:transparent;transition:.25s;font-family:inherit}
nav.anchors button:hover{color:var(--txt);border-color:var(--violet);background:rgba(124,58,237,.12)}
.stamp{color:var(--muted);font-size:11px;white-space:nowrap;padding-left:14px;border-left:1px solid var(--line)}
main{max-width:1480px;margin:auto;padding:34px 28px 90px}
section{margin-bottom:46px;scroll-margin-top:88px}
.sec-head{display:flex;align-items:flex-end;gap:14px;margin-bottom:18px;flex-wrap:wrap}
.sec-head h2{font-size:22px;margin:0;font-weight:700;letter-spacing:.4px}
.sec-head .rule{flex:1;height:1px;min-width:30px;background:linear-gradient(90deg,var(--line),transparent)}
.sec-head .sub{color:var(--muted);font-size:12px}
.glass{background:var(--glass);border:1px solid var(--line);border-radius:18px;backdrop-filter:blur(14px);box-shadow:0 18px 50px rgba(2,8,20,.38);padding:22px}
.hero{display:grid;grid-template-columns:1.45fr 1fr;gap:18px}
.hero .eyebrow{color:var(--teal);font-size:11px;letter-spacing:2.4px;text-transform:uppercase;margin:0 0 8px}
.hero h2.big{font-size:30px;line-height:1.25;margin:0;font-weight:700;background:linear-gradient(95deg,#ffffff,var(--sky) 55%,var(--teal));-webkit-background-clip:text;background-clip:text;color:transparent}
.hero .latin{font-style:italic;color:var(--muted);font-size:13px;margin:6px 0 0}
.badges{display:flex;gap:8px;flex-wrap:wrap;margin-top:16px}
.badge{font-size:11.5px;padding:5px 11px;border-radius:999px;border:1px solid rgba(20,184,166,.32);background:rgba(20,184,166,.09);color:#8ff0e3}
.badge.alt{border-color:rgba(124,58,237,.35);background:rgba(124,58,237,.10);color:#cbb4ff}
.badge.blue{border-color:rgba(56,189,248,.32);background:rgba(56,189,248,.09);color:#a9e2ff}
.kv{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:12px 18px}
.kv .item{border-left:2px solid rgba(20,184,166,.55);padding-left:12px}
.kv .item .lab{color:var(--muted);font-size:10.5px;text-transform:uppercase;letter-spacing:1.2px}
.kv .item .val{font-size:13.5px;word-break:break-word}
.cards{display:grid;grid-template-columns:repeat(6,minmax(0,1fr));gap:14px;margin-top:18px}
.card{background:var(--glass);border:1px solid var(--line);border-radius:15px;padding:16px 16px 14px;backdrop-filter:blur(12px);transition:.28s;animation:rise .55s both;position:relative;overflow:hidden}
.card:before{content:"";position:absolute;inset:0 0 auto 0;height:2px;background:linear-gradient(90deg,var(--teal),var(--blue),var(--violet));opacity:.75}
.card:hover{transform:translateY(-5px);border-color:rgba(20,184,166,.48);box-shadow:0 18px 42px rgba(20,184,166,.13)}
.card .k{color:var(--muted);font-size:11.5px;letter-spacing:.3px}
.card .v{font-size:25px;font-weight:700;line-height:1.3;background:linear-gradient(92deg,var(--teal),var(--sky));-webkit-background-clip:text;background-clip:text;color:transparent}
.card .u{font-size:10.5px;color:var(--muted)}
.card.warm .v{background:linear-gradient(92deg,var(--amber),var(--red));-webkit-background-clip:text;background-clip:text}
.card.violet .v{background:linear-gradient(92deg,var(--violet),var(--sky));-webkit-background-clip:text;background-clip:text}
.two{display:grid;grid-template-columns:1fr 1fr;gap:18px;margin-top:18px}
.glass h3{font-size:15px;margin:0 0 14px;font-weight:600;color:var(--txt)}
table.data{border-collapse:collapse;width:100%;font-size:12.5px}
table.data th{position:sticky;top:0;z-index:1;background:#16263c;color:var(--sky);text-align:left;padding:9px 10px;border-bottom:1px solid var(--line);white-space:nowrap;font-weight:600}
table.data td{padding:8px 10px;border-bottom:1px solid rgba(148,163,184,.08);vertical-align:top;max-width:340px;word-break:break-word}
table.data tbody tr{transition:background .2s}
table.data tbody tr:hover{background:rgba(56,189,248,.08)}
.table-wrap{max-height:440px;overflow:auto;border:1px solid var(--line);border-radius:12px;background:rgba(11,18,32,.35)}
details.panel{background:var(--glass);border:1px solid var(--line);border-radius:14px;margin-bottom:12px;overflow:hidden;transition:.25s;backdrop-filter:blur(10px)}
details.panel[open]{border-color:rgba(20,184,166,.34);box-shadow:0 12px 36px rgba(2,8,20,.38)}
details.panel summary{cursor:pointer;padding:13px 18px;display:flex;align-items:center;gap:12px;list-style:none;user-select:none;transition:background .2s}
details.panel summary:hover{background:rgba(20,184,166,.06)}
details.panel summary::-webkit-details-marker{display:none}
.chev{display:inline-block;width:8px;height:8px;border-right:2px solid var(--teal);border-bottom:2px solid var(--teal);transform:rotate(-45deg);transition:transform .25s}
details.panel[open] summary .chev{transform:rotate(45deg)}
.panel-title{font-weight:600;font-size:15px}
.panel-meta{margin-left:auto;color:var(--muted);font-size:11.5px;white-space:nowrap}
.panel-meta b{color:var(--teal);font-weight:700}
.panel-body{padding:2px 18px 18px;animation:fade .3s both}
.filter{width:100%;padding:9px 12px;margin-bottom:12px;border-radius:10px;border:1px solid var(--line);background:rgba(11,18,32,.72);color:var(--txt);outline:none;font-family:inherit;font-size:12.5px;transition:.2s}
.filter:focus{border-color:var(--teal);box-shadow:0 0 0 3px rgba(20,184,166,.14)}
.filter-row{display:flex;align-items:center;gap:12px;margin-bottom:12px}
.filter-row .filter{margin-bottom:0}
.filter-count{color:var(--muted);font-size:11px;white-space:nowrap}
.hint{color:var(--amber);font-size:11.5px;margin:10px 0 0}
.ell{color:var(--muted)}
.grid2{display:grid;grid-template-columns:1fr 1fr;gap:18px}
.figure{background:rgba(11,18,32,.42);border:1px solid var(--line);border-radius:16px;padding:18px;transition:.28s}
.figure:hover{border-color:rgba(56,189,248,.32);box-shadow:0 14px 40px rgba(2,8,20,.3)}
.figure h3{font-size:14px;margin:0 0 4px;font-weight:600}
.figure .cap{color:var(--muted);font-size:11.5px;margin:0 0 14px}
.figure svg{width:100%;height:auto;display:block}
.legend{display:flex;gap:16px;flex-wrap:wrap;color:var(--muted);font-size:11px;margin-top:12px}
.legend i{display:inline-block;width:10px;height:10px;border-radius:3px;margin-right:6px;vertical-align:middle}
.chip-list{display:flex;flex-wrap:wrap;gap:8px;margin-top:12px}
.chip{font-size:11px;padding:4px 9px;border-radius:8px;background:rgba(56,189,248,.09);border:1px solid rgba(56,189,248,.22);color:#bfe6ff}
footer{border-top:1px solid var(--line);padding:24px 28px 40px;color:var(--muted);font-size:11.5px}
.foot-in{max-width:1480px;margin:auto;display:flex;gap:18px;flex-wrap:wrap;align-items:center}
.foot-in .grow{flex:1}
@keyframes rise{from{opacity:0;transform:translateY(12px)}to{opacity:1;transform:none}}
@keyframes fade{from{opacity:0}to{opacity:1}}
@media (max-width:1180px){.cards{grid-template-columns:repeat(3,minmax(0,1fr))}.hero,.two,.grid2{grid-template-columns:1fr}}
@media (max-width:680px){.cards{grid-template-columns:repeat(2,minmax(0,1fr))}main{padding:22px 14px 60px}.top-in{padding:12px 14px}.hero h2.big{font-size:23px}nav.anchors{margin-left:0}.kv{grid-template-columns:1fr}}
          </xsl:text>
        </style>
        <script>
          <xsl:text>
function filterRows(input){
  var body = input.closest('.panel-body');
  var rows = body.querySelectorAll('tbody tr');
  var q = input.value.trim().toLowerCase();
  var shown = 0;
  Array.prototype.forEach.call(rows,function(row){
    var text = row.textContent.toLowerCase();
    var hit = text.indexOf(q) !== -1;
    row.style.display = hit ? '' : 'none';
    if(hit){shown = shown + 1;}
  });
  var badge = body.querySelector('.filter-count');
  if(badge){badge.textContent = shown + ' / ' + rows.length;}
}
function toggleAll(open){
  var panels = document.querySelectorAll('details.panel');
  Array.prototype.forEach.call(panels,function(p){p.open = open;});
}
          </xsl:text>
        </script>
      </head>
      <body>
        <header class="top">
          <div class="top-in">
            <div class="brand">
              <div class="logo">VC</div>
              <div>
                <h1><xsl:value-of select="$reportTitle"/></h1>
                <p>GCModeller · GCMarkup <xsl:value-of select="$vc/@schemaVersion"/> · Virtual Cell</p>
              </div>
            </div>
            <nav class="anchors">
              <a href="#overview">概览</a>
              <a href="#graphics">图形</a>
              <a href="#details">明细</a>
              <a href="#about">关于</a>
              <button type="button" onclick="toggleAll(true)">展开全部</button>
              <button type="button" onclick="toggleAll(false)">收起全部</button>
            </nav>
            <div class="stamp"><xsl:value-of select="$generatedAt"/></div>
          </div>
        </header>

        <main>
          <xsl:call-template name="dashboard"/>
          <xsl:call-template name="graphics"/>
          <xsl:call-template name="details"/>
        </main>

        <footer id="about">
          <div class="foot-in">
            <div>
              <div><b>数据源</b> <xsl:value-of select="$vc/@dataSource"/></div>
              <div><b>Schema</b> GCMarkup <xsl:value-of select="$vc/@schemaVersion"/></div>
              <div><b>模型 GUID</b> <xsl:value-of select="$vc/vck:properties/vck:guid"/></div>
            </div>
            <div>
              <div><b>细胞标识</b> <xsl:value-of select="$vc/gcm:cellular_id"/></div>
              <div><b>编译时间</b> <xsl:value-of select="$vc/vck:properties/@compiled"/></div>
              <div><b>报告生成</b> <xsl:value-of select="$generatedAt"/></div>
            </div>
            <div class="grow">
              <div><b>限量说明</b> 每个明细分区仅渲染前 <xsl:value-of select="$limit"/> 条记录，面板标题给出该分区的完整总数；完整数据请以模型 XML 为准。</div>
              <div style="margin-top:6px">本页为离线生成的静态快照，样式与脚本全部内联，无外部网络请求。</div>
            </div>
            <a href="#overview">返回顶部 ↑</a>
          </div>
        </footer>
      </body>
    </html>
  </xsl:template>

  <!-- ================================================================== -->
  <!-- Overview dashboard                                                  -->
  <!-- ================================================================== -->
  <xsl:template name="dashboard">
    <section id="overview">
      <div class="sec-head">
        <h2>模型概览</h2><div class="rule"><xsl:text> </xsl:text></div><div class="sub">Model identity and composition</div>
      </div>

      <div class="hero">
        <div class="glass">
          <p class="eyebrow">Virtual Cell Model</p>
          <h2 class="big">
            <xsl:choose>
              <xsl:when test="$vc/vck:properties/vck:title"><xsl:value-of select="$vc/vck:properties/vck:title"/></xsl:when>
              <xsl:when test="$vc/gcm:taxonomy/@scientificName"><xsl:value-of select="$vc/gcm:taxonomy/@scientificName"/></xsl:when>
              <xsl:otherwise><xsl:value-of select="$vc/vck:properties/@name"/></xsl:otherwise>
            </xsl:choose>
          </h2>
          <p class="latin">
            <xsl:value-of select="$vc/gcm:taxonomy/@scientificName"/>
            <xsl:if test="$vc/gcm:taxonomy/gcmod:species"> · <xsl:value-of select="$vc/gcm:taxonomy/gcmod:species"/></xsl:if>
          </p>
          <div class="badges">
            <span class="badge">NCBI TaxID <xsl:value-of select="$vc/gcm:taxonomy/@ncbi_taxid"/></span>
            <span class="badge blue">Cellular ID <xsl:value-of select="$vc/gcm:cellular_id"/></span>
            <span class="badge alt">schema <xsl:value-of select="$vc/@schemaVersion"/></span>
            <span class="badge alt">reversion <xsl:value-of select="$vc/vck:properties/@reversion"/></span>
          </div>
          <div class="kv" style="margin-top:20px">
            <div class="item"><div class="lab">数据源</div><div class="val"><xsl:value-of select="$vc/@dataSource"/></div></div>
            <div class="item"><div class="lab">编译时间</div><div class="val"><xsl:value-of select="$vc/vck:properties/@compiled"/></div></div>
            <div class="item"><div class="lab">基因组大小</div><div class="val"><xsl:value-of select="format-number(number($genome/@genomeSize) div 1000000, '0.00')"/> Mb (<xsl:value-of select="format-number(number($genome/@genomeSize), '#,##0')"/> bp)</div></div>
            <div class="item"><div class="lab">GC 含量</div><div class="val"><xsl:value-of select="format-number(number($genome/@gcContent) * 100, '0.00')"/> %</div></div>
            <div class="item"><div class="lab">复制子</div><div class="val"><xsl:value-of select="count($replicons)"/> 个（染色体 <xsl:value-of select="count($replicons[@isPlasmid='false'])"/> / 质粒 <xsl:value-of select="count($replicons[@isPlasmid='true'])"/>）</div></div>
            <div class="item"><div class="lab">模型 GUID</div><div class="val"><xsl:value-of select="$vc/vck:properties/vck:guid"/></div></div>
          </div>
        </div>

        <div class="glass">
          <h3>生理状态与培养条件</h3>
          <div class="kv">
            <div class="item"><div class="lab">温度</div><div class="val"><xsl:value-of select="$vc/gcm:physiology/@temperature"/> °C</div></div>
            <div class="item"><div class="lab">pH</div><div class="val"><xsl:value-of select="$vc/gcm:physiology/@pH"/></div></div>
            <div class="item"><div class="lab">倍增时间</div><div class="val"><xsl:value-of select="$vc/gcm:physiology/@doublingTime"/></div></div>
            <div class="item"><div class="lab">生长速率</div><div class="val"><xsl:value-of select="$vc/gcm:physiology/@growthRate"/></div></div>
            <div class="item"><div class="lab">需氧性</div><div class="val"><xsl:value-of select="$vc/gcm:physiology/@oxygenRequirement"/></div></div>
            <div class="item"><div class="lab">离子强度</div><div class="val"><xsl:value-of select="$vc/gcm:traits/@oxygenRequirement"/></div></div>
          </div>
          <xsl:if test="$vc/gcm:physiology/gcm:medium">
            <h3 style="margin-top:20px">培养基 / 模型说明</h3>
            <p style="color:var(--muted);font-size:12px;max-height:120px;overflow:auto;margin:0">
              <xsl:call-template name="plain">
                <xsl:with-param name="s" select="$vc/gcm:physiology/gcm:medium"/>
                <xsl:with-param name="n" select="1200"/>
              </xsl:call-template>
            </p>
          </xsl:if>
          <xsl:if test="$vc/vck:properties/vck:URLs">
            <div class="chip-list">
              <xsl:for-each select="$vc/vck:properties/vck:URLs">
                <span class="chip"><xsl:value-of select="."/></span>
              </xsl:for-each>
            </div>
          </xsl:if>
        </div>
      </div>

      <div class="cards">
        <xsl:call-template name="stat-card"><xsl:with-param name="k" select="'基因'"/><xsl:with-param name="v" select="count($genes)"/><xsl:with-param name="u" select="'genes / CDS'"/></xsl:call-template>
        <xsl:call-template name="stat-card"><xsl:with-param name="k" select="'转录单元'"/><xsl:with-param name="v" select="count($tus)"/><xsl:with-param name="u" select="'TranscriptUnit'"/></xsl:call-template>
        <xsl:call-template name="stat-card"><xsl:with-param name="k" select="'转录调控'"/><xsl:with-param name="v" select="count($regulations)"/><xsl:with-param name="u" select="'regulations'"/></xsl:call-template>
        <xsl:call-template name="stat-card"><xsl:with-param name="k" select="'蛋白质'"/><xsl:with-param name="v" select="count($proteins)"/><xsl:with-param name="u" select="'proteins'"/></xsl:call-template>
        <xsl:call-template name="stat-card"><xsl:with-param name="k" select="'化合物'"/><xsl:with-param name="v" select="count($compounds)"/><xsl:with-param name="u" select="'compounds'"/></xsl:call-template>
        <xsl:call-template name="stat-card"><xsl:with-param name="k" select="'代谢反应'"/><xsl:with-param name="v" select="$reactionTotal"/><xsl:with-param name="u" select="'enzymatic + spontaneous'"/></xsl:call-template>
        <xsl:call-template name="stat-card"><xsl:with-param name="k" select="'转运过程'"/><xsl:with-param name="v" select="count($transportations)"/><xsl:with-param name="u" select="'transportation'"/></xsl:call-template>
        <xsl:call-template name="stat-card"><xsl:with-param name="k" select="'酶'"/><xsl:with-param name="v" select="count($enzymes)"/><xsl:with-param name="u" select="'enzymes'"/></xsl:call-template>
        <xsl:call-template name="stat-card"><xsl:with-param name="k" select="'代谢通路'"/><xsl:with-param name="v" select="count($pathways)"/><xsl:with-param name="u" select="'pathways'"/></xsl:call-template>
        <xsl:call-template name="stat-card"><xsl:with-param name="k" select="'文献证据'"/><xsl:with-param name="v" select="count($publications)"/><xsl:with-param name="u" select="'publications'"/></xsl:call-template>
        <xsl:call-template name="stat-card"><xsl:with-param name="k" select="'分类术语'"/><xsl:with-param name="v" select="count($classTerms)"/><xsl:with-param name="u" select="'class terms'"/></xsl:call-template>
        <xsl:call-template name="stat-card"><xsl:with-param name="k" select="'区室'"/><xsl:with-param name="v" select="count($compartments)"/><xsl:with-param name="u" select="'compartments'"/></xsl:call-template>
      </div>

      <div class="two">
        <div class="glass">
          <h3>区室定义表</h3>
          <div class="table-wrap" style="max-height:260px">
            <table class="data">
              <thead><tr><th>ID</th><th>名称</th><th>类型</th><th>体积</th><th>pH</th><th>离子强度</th></tr></thead>
              <tbody>
                <xsl:for-each select="$compartments">
                  <tr>
                    <td><xsl:value-of select="@id"/></td>
                    <td><xsl:value-of select="@name"/></td>
                    <td><xsl:value-of select="@kind"/></td>
                    <td><xsl:value-of select="@volume"/></td>
                    <td><xsl:value-of select="@pH"/></td>
                    <td><xsl:value-of select="@ionicStrength"/></td>
                  </tr>
                </xsl:for-each>
              </tbody>
            </table>
          </div>
        </div>
        <div class="glass">
          <h3>网络组成分布</h3>
          <div class="kv">
            <div class="item"><div class="lab">酶促反应</div><div class="val"><xsl:value-of select="format-number(count($enzymaticReactions), '#,##0')"/></div></div>
            <div class="item"><div class="lab">非酶促反应</div><div class="val"><xsl:value-of select="format-number(count($nonEnzymaticReactions), '#,##0')"/></div></div>
            <div class="item"><div class="lab">启动子</div><div class="val"><xsl:value-of select="format-number(count($promoters), '#,##0')"/></div></div>
            <div class="item"><div class="lab">终止子</div><div class="val"><xsl:value-of select="format-number(count($terminators), '#,##0')"/></div></div>
            <div class="item"><div class="lab">结合位点</div><div class="val"><xsl:value-of select="format-number(count($bindingSites), '#,##0')"/></div></div>
            <div class="item"><div class="lab">调控子</div><div class="val"><xsl:value-of select="format-number(count($regulons), '#,##0')"/></div></div>
            <div class="item"><div class="lab">RNA 基因</div><div class="val"><xsl:value-of select="format-number(count($rnas), '#,##0')"/></div></div>
            <div class="item"><div class="lab">蛋白特征</div><div class="val"><xsl:value-of select="format-number(count($proteinFeatures), '#,##0')"/></div></div>
            <div class="item"><div class="lab">通路类别</div><div class="val"><xsl:value-of select="format-number(count($categories), '#,##0')"/></div></div>
            <div class="item"><div class="lab">作者 / 数据库链接</div><div class="val"><xsl:value-of select="count($vc/vck:properties/vck:authors)"/> / <xsl:value-of select="count($vc/vck:properties/vck:DBLinks/vck:string)"/></div></div>
          </div>
        </div>
      </div>
    </section>
  </xsl:template>

  <xsl:template name="noop"/>

  <xsl:template name="stat-card">
    <xsl:param name="k"/>
    <xsl:param name="v"/>
    <xsl:param name="u"/>
    <div class="card">
      <div class="k"><xsl:value-of select="$k"/></div>
      <div class="v"><xsl:value-of select="format-number(number($v), '#,##0')"/></div>
      <div class="u"><xsl:value-of select="$u"/></div>
    </div>
  </xsl:template>

  <!-- ================================================================== -->
  <!-- Shared helpers                                                      -->
  <!-- ================================================================== -->

  <!-- Remove escaped inline HTML tags such as &lt;i&gt; from model text. -->
  <xsl:template name="strip-tags">
    <xsl:param name="text"/>
    <xsl:choose>
      <xsl:when test="contains($text, '&lt;') and contains(substring-after($text, '&lt;'), '&gt;')">
        <xsl:value-of select="substring-before($text, '&lt;')"/>
        <xsl:call-template name="strip-tags">
          <xsl:with-param name="text" select="substring-after(substring-after($text, '&lt;'), '&gt;')"/>
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise><xsl:value-of select="$text"/></xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <!-- Plain, tag-free, length-limited text. -->
  <xsl:template name="plain">
    <xsl:param name="s"/>
    <xsl:param name="n" select="160"/>
    <xsl:variable name="clean">
      <xsl:call-template name="strip-tags">
        <xsl:with-param name="text" select="string($s)"/>
      </xsl:call-template>
    </xsl:variable>
    <xsl:choose>
      <xsl:when test="string-length($clean) &gt; number($n)">
        <xsl:value-of select="substring($clean, 1, number($n))"/><xsl:text> …</xsl:text>
      </xsl:when>
      <xsl:otherwise><xsl:value-of select="$clean"/></xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <!-- A table cell with full text in the tooltip. -->
  <xsl:template name="td">
    <xsl:param name="v"/>
    <xsl:param name="n" select="150"/>
    <xsl:variable name="clean">
      <xsl:call-template name="strip-tags">
        <xsl:with-param name="text" select="string($v)"/>
      </xsl:call-template>
    </xsl:variable>
    <td>
      <xsl:attribute name="title"><xsl:value-of select="substring($clean, 1, 500)"/></xsl:attribute>
      <xsl:choose>
        <xsl:when test="string-length($clean) &gt; number($n)">
          <xsl:value-of select="substring($clean, 1, number($n))"/><span class="ell"> …</span>
        </xsl:when>
        <xsl:otherwise><xsl:value-of select="$clean"/></xsl:otherwise>
      </xsl:choose>
    </td>
  </xsl:template>

  <!-- Column headers from a pipe separated string. -->
  <xsl:template name="headers">
    <xsl:param name="cols"/>
    <xsl:choose>
      <xsl:when test="contains($cols, '|')">
        <th><xsl:value-of select="substring-before($cols, '|')"/></th>
        <xsl:call-template name="headers">
          <xsl:with-param name="cols" select="substring-after($cols, '|')"/>
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise><th><xsl:value-of select="$cols"/></th></xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <!-- Maximum of a whitespace separated number list. -->
  <xsl:template name="max-of">
    <xsl:param name="nums"/>
    <xsl:param name="cur" select="0"/>
    <xsl:variable name="list" select="normalize-space($nums)"/>
    <xsl:choose>
      <xsl:when test="$list = ''"><xsl:value-of select="$cur"/></xsl:when>
      <xsl:otherwise>
        <xsl:variable name="head" select="number(substring-before(concat($list, ' '), ' '))"/>
        <xsl:call-template name="max-of">
          <xsl:with-param name="nums" select="substring-after($list, ' ')"/>
          <xsl:with-param name="cur">
            <xsl:choose>
              <xsl:when test="$head &gt; number($cur)"><xsl:value-of select="$head"/></xsl:when>
              <xsl:otherwise><xsl:value-of select="$cur"/></xsl:otherwise>
            </xsl:choose>
          </xsl:with-param>
        </xsl:call-template>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <!-- Generic collapsible, filterable, limited detail panel.
       Rows are rendered by the mode="row" templates below. -->
  <xsl:template name="panel">
    <xsl:param name="id"/>
    <xsl:param name="title"/>
    <xsl:param name="cols"/>
    <xsl:param name="rows"/>
    <xsl:param name="note" select="''"/>
    <xsl:variable name="shown" select="count($rows[position() &lt;= $limit])"/>
    <details class="panel">
      <xsl:attribute name="id"><xsl:value-of select="$id"/></xsl:attribute>
      <summary>
        <span class="chev"><xsl:text> </xsl:text></span>
        <span class="panel-title"><xsl:value-of select="$title"/></span>
        <span class="panel-meta">已显示 <b><xsl:value-of select="$shown"/></b> / 共 <b><xsl:value-of select="format-number(count($rows), '#,##0')"/></b> 条</span>
      </summary>
      <div class="panel-body">
        <xsl:if test="$note != ''"><p class="hint" style="margin-top:0"><xsl:value-of select="$note"/></p></xsl:if>
        <div class="filter-row">
          <input class="filter" type="text" oninput="filterRows(this)" placeholder="输入关键词过滤已加载的记录 …">
            <xsl:attribute name="aria-label"><xsl:value-of select="$title"/> 过滤</xsl:attribute>
          </input>
          <span class="filter-count"><xsl:value-of select="$shown"/> / <xsl:value-of select="$shown"/></span>
        </div>
        <div class="table-wrap">
          <table class="data">
            <thead><tr><xsl:call-template name="headers"><xsl:with-param name="cols" select="$cols"/></xsl:call-template></tr></thead>
            <tbody>
              <xsl:for-each select="$rows[position() &lt;= $limit]">
                <xsl:apply-templates select="." mode="row"/>
              </xsl:for-each>
            </tbody>
          </table>
        </div>
        <xsl:if test="count($rows) &gt; number($limit)">
          <p class="hint">为避免浏览器卡顿，此处仅渲染前 <xsl:value-of select="$limit"/> 条；该分区实际共 <xsl:value-of select="format-number(count($rows), '#,##0')"/> 条记录。</p>
        </xsl:if>
      </div>
    </details>
  </xsl:template>

  <!-- ================================================================== -->
  <!-- Graphics / details sections: filled in by the following templates.   -->
  <!-- ================================================================== -->
  <xsl:template name="graphics">
    <section id="graphics">
      <div class="sec-head">
        <h2>图形化视图</h2><div class="rule"><xsl:text> </xsl:text></div><div class="sub">Genome track · Composition · Metabolic network</div>
      </div>
      <xsl:call-template name="genome-tracks"/>

      <div class="grid2" style="margin-top:18px">
        <div><xsl:call-template name="composition-chart"/></div>
        <div><xsl:call-template name="category-chart"/></div>
      </div>

      <div class="grid2" style="margin-top:18px">
        <div><xsl:call-template name="class-chart"/></div>
        <div><xsl:call-template name="year-chart"/></div>
      </div>

      <div style="margin-top:18px"><xsl:call-template name="metabolic-network"/></div>
    </section>
  </xsl:template>

  <xsl:template name="details">
    <section id="details">
      <div class="sec-head">
        <h2>分区明细</h2><div class="rule"><xsl:text> </xsl:text></div><div class="sub">折叠面板 · 关键词过滤 · 限量展示</div>
      </div>
      <h3 style="font-size:12px;letter-spacing:1.6px;color:var(--sky);text-transform:uppercase;margin:22px 0 10px">A · 基因组结构 / Genome</h3>

      <xsl:call-template name="panel">
        <xsl:with-param name="id" select="'sec-replicons'"/>
        <xsl:with-param name="title" select="'复制子 Replicons'"/>
        <xsl:with-param name="cols" select="'名称|类型|拓扑|长度 (bp)|GC %|拷贝数|转录单元'"/>
        <xsl:with-param name="rows" select="$replicons"/>
      </xsl:call-template>
      <xsl:call-template name="panel">
        <xsl:with-param name="id" select="'sec-tus'"/>
        <xsl:with-param name="title" select="'转录单元 TranscriptUnit'"/>
        <xsl:with-param name="cols" select="'ID|名称|方向|调控位点|基因数|基因列表|启动子|调控关系'"/>
        <xsl:with-param name="rows" select="$tus"/>
      </xsl:call-template>
      <xsl:call-template name="panel">
        <xsl:with-param name="id" select="'sec-genes'"/>
        <xsl:with-param name="title" select="'基因 Genes'"/>
        <xsl:with-param name="cols" select="'locus_tag|基因名|accession|链|起始|终止|类型|产物|centisome|所属单元'"/>
        <xsl:with-param name="rows" select="$genes"/>
      </xsl:call-template>
      <xsl:call-template name="panel">
        <xsl:with-param name="id" select="'sec-rnas'"/>
        <xsl:with-param name="title" select="'RNA 基因'"/>
        <xsl:with-param name="cols" select="'ID|基因|类型|val|componentOf|终止位置'"/>
        <xsl:with-param name="rows" select="$rnas"/>
      </xsl:call-template>

      <h3 style="font-size:12px;letter-spacing:1.6px;color:var(--sky);text-transform:uppercase;margin:26px 0 10px">B · 转录调控网络 / Regulation</h3>

      <xsl:call-template name="panel">
        <xsl:with-param name="id" select="'sec-regulations'"/>
        <xsl:with-param name="title" select="'转录调控 Transcription regulations'"/>
        <xsl:with-param name="cols" select="'调控因子|模式|操纵子|Ki|motif 家族|证据|中心法则目标'"/>
        <xsl:with-param name="rows" select="$regulations"/>
      </xsl:call-template>
      <xsl:call-template name="panel">
        <xsl:with-param name="id" select="'sec-regulons'"/>
        <xsl:with-param name="title" select="'调控子 Regulons'"/>
        <xsl:with-param name="cols" select="'ID|名称|调控因子'"/>
        <xsl:with-param name="rows" select="$regulons"/>
      </xsl:call-template>
      <xsl:call-template name="panel">
        <xsl:with-param name="id" select="'sec-bindingsites'"/>
        <xsl:with-param name="title" select="'DNA 结合位点 Binding sites'"/>
        <xsl:with-param name="cols" select="'ID|操纵子|起始|终止|得分|位点类型'"/>
        <xsl:with-param name="rows" select="$bindingSites"/>
      </xsl:call-template>
      <xsl:call-template name="panel">
        <xsl:with-param name="id" select="'sec-promoters'"/>
        <xsl:with-param name="title" select="'启动子 Promoters'"/>
        <xsl:with-param name="cols" select="'ID|操纵子|链|TSS|sigma 因子|minus10 区|minus35 区'"/>
        <xsl:with-param name="rows" select="$promoters"/>
      </xsl:call-template>
      <xsl:call-template name="panel">
        <xsl:with-param name="id" select="'sec-terminators'"/>
        <xsl:with-param name="title" select="'终止子 Terminators'"/>
        <xsl:with-param name="cols" select="'ID|操纵子|起始|终止|位置'"/>
        <xsl:with-param name="rows" select="$terminators"/>
      </xsl:call-template>

      <h3 style="font-size:12px;letter-spacing:1.6px;color:var(--sky);text-transform:uppercase;margin:26px 0 10px">C · 蛋白质与特征 / Proteome</h3>

      <xsl:call-template name="panel">
        <xsl:with-param name="id" select="'sec-proteins'"/>
        <xsl:with-param name="title" select="'蛋白质 Proteins'"/>
        <xsl:with-param name="cols" select="'protein_id|名称|分子量|pI|细胞定位|复合物|GO 注释数'"/>
        <xsl:with-param name="rows" select="$proteins"/>
      </xsl:call-template>
      <xsl:call-template name="panel">
        <xsl:with-param name="id" select="'sec-proteinfeatures'"/>
        <xsl:with-param name="title" select="'蛋白特征 Protein features'"/>
        <xsl:with-param name="cols" select="'ID|蛋白|特征类型|起始|终止|说明'"/>
        <xsl:with-param name="rows" select="$proteinFeatures"/>
      </xsl:call-template>

      <h3 style="font-size:12px;letter-spacing:1.6px;color:var(--sky);text-transform:uppercase;margin:26px 0 10px">D · 代谢网络 / Metabolome</h3>

      <xsl:call-template name="panel">
        <xsl:with-param name="id" select="'sec-compounds'"/>
        <xsl:with-param name="title" select="'化合物 Compounds'"/>
        <xsl:with-param name="cols" select="'ID|名称|分子式|质量|电荷|gibbs0|分类|SMILES|同义名'"/>
        <xsl:with-param name="rows" select="$compounds"/>
        <xsl:with-param name="note" select="'化合物名称为混合内容文本节点，SMILES 与同义名均已截断显示，完整值见鼠标悬停提示。'"/>
      </xsl:call-template>
      <xsl:call-template name="panel">
        <xsl:with-param name="id" select="'sec-reactions'"/>
        <xsl:with-param name="title" select="'代谢反应 Reactions'"/>
        <xsl:with-param name="cols" select="'ID|酶促|方向|bounds|ΔG|名称|反应式|EC 号|通路|酶'"/>
        <xsl:with-param name="rows" select="$reactionNodes"/>
        <xsl:with-param name="note" select="'酶促与非酶促反应合并展示；转运过程在下一面板单独列出。'"/>
      </xsl:call-template>
      <xsl:call-template name="panel">
        <xsl:with-param name="id" select="'sec-transport'"/>
        <xsl:with-param name="title" select="'跨膜转运 Transportation'"/>
        <xsl:with-param name="cols" select="'GUID|膜结构|偶联类型|酶|底物|产物|反应 ID'"/>
        <xsl:with-param name="rows" select="$transportations"/>
      </xsl:call-template>
      <xsl:call-template name="panel">
        <xsl:with-param name="id" select="'sec-enzymes'"/>
        <xsl:with-param name="title" select="'酶 Enzymes'"/>
        <xsl:with-param name="cols" select="'proteinID|kcat|Km|vmax|kcatKm|方向|催化反应|动力学公式'"/>
        <xsl:with-param name="rows" select="$enzymes"/>
      </xsl:call-template>
      <xsl:call-template name="panel">
        <xsl:with-param name="id" select="'sec-pathways'"/>
        <xsl:with-param name="title" select="'代谢通路 Pathways'"/>
        <xsl:with-param name="cols" select="'ID|名称|类别|classId|反应数|说明'"/>
        <xsl:with-param name="rows" select="$pathways"/>
      </xsl:call-template>
      <xsl:call-template name="panel">
        <xsl:with-param name="id" select="'sec-categories'"/>
        <xsl:with-param name="title" select="'通路功能分类 FunctionalCategory'"/>
        <xsl:with-param name="cols" select="'类别|通路数|示例通路|首个通路 ID'"/>
        <xsl:with-param name="rows" select="$categories"/>
      </xsl:call-template>

      <h3 style="font-size:12px;letter-spacing:1.6px;color:var(--sky);text-transform:uppercase;margin:26px 0 10px">E · 注释与证据 / Annotation</h3>

      <xsl:call-template name="panel">
        <xsl:with-param name="id" select="'sec-evidence'"/>
        <xsl:with-param name="title" select="'文献证据 Publications'"/>
        <xsl:with-param name="cols" select="'ID|PubMed|年份|DOI|标题|首位作者|来源'"/>
        <xsl:with-param name="rows" select="$publications"/>
      </xsl:call-template>
      <xsl:call-template name="panel">
        <xsl:with-param name="id" select="'sec-classterms'"/>
        <xsl:with-param name="title" select="'BioCyc / GO 分类术语 ClassTerms'"/>
        <xsl:with-param name="cols" select="'ID|名称|父节点|缩写'"/>
        <xsl:with-param name="rows" select="$classTerms"/>
      </xsl:call-template>
      <xsl:call-template name="panel">
        <xsl:with-param name="id" select="'sec-metadata'"/>
        <xsl:with-param name="title" select="'模型元数据 vcellkit:properties'"/>
        <xsl:with-param name="cols" select="'字段|值'"/>
        <xsl:with-param name="rows" select="$vc/vck:properties/vck:authors | $vc/vck:properties/vck:publications | $vc/vck:properties/vck:URLs | $vc/vck:properties/vck:DBLinks/vck:string"/>
      </xsl:call-template>
    </section>
  </xsl:template>

  <!-- ================================================================== -->
  <!-- Row templates (mode="row"), one per section item element.            -->
  <!-- Column counts MUST match the cols parameter of the panel above.      -->
  <!-- ================================================================== -->

  <xsl:template match="gcm:replicon" mode="row">
    <tr>
      <td><xsl:value-of select="@genomeName"/></td>
      <td><xsl:choose><xsl:when test="@isPlasmid='true'">质粒</xsl:when><xsl:otherwise>染色体</xsl:otherwise></xsl:choose></td>
      <td><xsl:value-of select="@topology"/></td>
      <td><xsl:value-of select="format-number(number(@length), '#,##0')"/></td>
      <td><xsl:value-of select="format-number(number(@gcContent) * 100, '0.00')"/> %</td>
      <td><xsl:value-of select="@copyNumber"/></td>
      <td><xsl:value-of select="count(gcm:operons/gcm:TranscriptUnit)"/></td>
    </tr>
  </xsl:template>

  <xsl:template match="gcm:TranscriptUnit" mode="row">
    <tr>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@id"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@name"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@direction"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@sites"/><xsl:with-param name="n" select="70"/></xsl:call-template>
      <td><xsl:value-of select="count(gcm:genes)"/></td>
      <td>
        <xsl:for-each select="gcm:genes[position() &lt;= 8]">
          <xsl:if test="position() != 1"><xsl:text>, </xsl:text></xsl:if>
          <xsl:value-of select="@name"/>
        </xsl:for-each>
        <xsl:if test="count(gcm:genes) &gt; 8"><xsl:text> …</xsl:text></xsl:if>
      </td>
      <xsl:call-template name="td"><xsl:with-param name="v" select="gcm:promoter/@id"/></xsl:call-template>
      <td><xsl:value-of select="count(gcm:regulatedBy)"/></td>
    </tr>
  </xsl:template>

  <xsl:template match="gcm:genes" mode="row">
    <tr>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@locus_tag"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@name"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@accession"/></xsl:call-template>
      <td><xsl:value-of select="@strand"/></td>
      <td><xsl:value-of select="format-number(number(@left), '#,##0')"/></td>
      <td><xsl:value-of select="format-number(number(@right), '#,##0')"/></td>
      <td><xsl:value-of select="@type"/></td>
      <xsl:call-template name="td"><xsl:with-param name="v" select="gcm:product"/></xsl:call-template>
      <td><xsl:value-of select="@centisomePosition"/></td>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@componentOf"/></xsl:call-template>
    </tr>
  </xsl:template>

  <xsl:template match="gcm:RNA" mode="row">
    <tr>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@id"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@gene"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@type"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@val"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@componentOf"/></xsl:call-template>
      <td><xsl:value-of select="@endPosition"/></td>
    </tr>
  </xsl:template>

  <xsl:template match="gcm:transcription" mode="row">
    <tr>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@regulator"/></xsl:call-template>
      <td><xsl:value-of select="@mode"/></td>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@operonId"/></xsl:call-template>
      <td><xsl:value-of select="@ki"/></td>
      <xsl:call-template name="td"><xsl:with-param name="v" select="gcm:motif/@family"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="gcm:evidence"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="gcm:centralDogma/gcm:string"/></xsl:call-template>
    </tr>
  </xsl:template>

  <xsl:template match="gcm:regulon" mode="row">
    <tr>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@id"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@name"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@regulator"/></xsl:call-template>
    </tr>
  </xsl:template>

  <xsl:template match="gcm:bindingSite" mode="row">
    <tr>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@id"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@operonId"/></xsl:call-template>
      <td><xsl:value-of select="@left"/></td>
      <td><xsl:value-of select="@right"/></td>
      <td><xsl:value-of select="@score"/></td>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@siteType"/></xsl:call-template>
    </tr>
  </xsl:template>

  <xsl:template match="gcm:promoter" mode="row">
    <tr>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@id"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@operonId"/></xsl:call-template>
      <td><xsl:value-of select="@strand"/></td>
      <td><xsl:value-of select="@tss"/></td>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@sigmaFactor"/></xsl:call-template>
      <td><xsl:value-of select="@minus10Left"/> - <xsl:value-of select="@minus10Right"/></td>
      <td><xsl:value-of select="@minus35Left"/> - <xsl:value-of select="@minus35Right"/></td>
    </tr>
  </xsl:template>

  <xsl:template match="gcm:terminator" mode="row">
    <tr>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@id"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@operonId"/></xsl:call-template>
      <td><xsl:value-of select="@left"/></td>
      <td><xsl:value-of select="@right"/></td>
      <td><xsl:value-of select="@position"/></td>
    </tr>
  </xsl:template>

  <xsl:template match="gcm:protein" mode="row">
    <tr>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@protein_id"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@name"/><xsl:with-param name="n" select="120"/></xsl:call-template>
      <td><xsl:value-of select="format-number(number(@molecularWeight), '#,##0.00')"/></td>
      <td><xsl:value-of select="@pi"/></td>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@cellular_location"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@complexId"/></xsl:call-template>
      <td><xsl:value-of select="count(gcm:go)"/></td>
    </tr>
  </xsl:template>

  <xsl:template match="gcm:proteinFeature" mode="row">
    <tr>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@id"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@protein"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@featureType"/></xsl:call-template>
      <td><xsl:value-of select="@left"/></td>
      <td><xsl:value-of select="@right"/></td>
      <xsl:call-template name="td"><xsl:with-param name="v" select="gcm:note"/><xsl:with-param name="n" select="180"/></xsl:call-template>
    </tr>
  </xsl:template>

  <xsl:template match="gcm:compound" mode="row">
    <tr>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@ID"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="text()[1]"/><xsl:with-param name="n" select="120"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@formula"/></xsl:call-template>
      <td><xsl:value-of select="@mass"/></td>
      <td><xsl:value-of select="@charge"/></td>
      <td><xsl:value-of select="@gibbs0"/></td>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@classId"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="gcm:smiles"/><xsl:with-param name="n" select="60"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="gcm:synonym[1]"/><xsl:with-param name="n" select="90"/></xsl:call-template>
    </tr>
  </xsl:template>

  <xsl:template match="gcm:reaction" mode="row">
    <tr>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@ID"/></xsl:call-template>
      <td><xsl:choose><xsl:when test="@is_enzymatic='true'">酶促</xsl:when><xsl:otherwise>非酶促</xsl:otherwise></xsl:choose></td>
      <td><xsl:value-of select="@direction"/></td>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@bounds"/></xsl:call-template>
      <td><xsl:value-of select="@deltaG"/></td>
      <xsl:call-template name="td"><xsl:with-param name="v" select="gcm:name"/><xsl:with-param name="n" select="110"/></xsl:call-template>
      <td>
        <xsl:for-each select="gcm:substrate">
          <xsl:if test="position() != 1"><xsl:text> + </xsl:text></xsl:if>
          <xsl:call-template name="plain"><xsl:with-param name="s" select="."/><xsl:with-param name="n" select="34"/></xsl:call-template>
        </xsl:for-each>
        <xsl:text>  →  </xsl:text>
        <xsl:for-each select="gcm:product">
          <xsl:if test="position() != 1"><xsl:text> + </xsl:text></xsl:if>
          <xsl:call-template name="plain"><xsl:with-param name="s" select="."/><xsl:with-param name="n" select="34"/></xsl:call-template>
        </xsl:for-each>
      </td>
      <xsl:call-template name="td"><xsl:with-param name="v" select="gcm:ec_number"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="gcm:pathway"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="gcm:enzyme"/></xsl:call-template>
    </tr>
  </xsl:template>

  <xsl:template match="gcm:Transportation" mode="row">
    <tr>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@guid"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@membrane"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@couplingType"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="gcm:enzymes"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="gcm:substrate"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="gcm:product"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="gcm:reaction"/></xsl:call-template>
    </tr>
  </xsl:template>

  <xsl:template match="gcm:enzyme" mode="row">
    <tr>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@proteinID"/></xsl:call-template>
      <td><xsl:value-of select="@kcat"/></td>
      <td><xsl:value-of select="@km"/></td>
      <td><xsl:value-of select="@vmax"/></td>
      <td><xsl:value-of select="@kcatKm"/></td>
      <td><xsl:value-of select="@direction"/></td>
      <xsl:call-template name="td"><xsl:with-param name="v" select="gcm:catalysis/@reaction"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="gcm:catalysis/gcm:formula/@name"/><xsl:with-param name="n" select="120"/></xsl:call-template>
    </tr>
  </xsl:template>

  <xsl:template match="gcm:pathway" mode="row">
    <tr>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@ID"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@name"/><xsl:with-param name="n" select="130"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@category"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@classId"/></xsl:call-template>
      <td><xsl:value-of select="count(gcm:reactions/gcm:string)"/></td>
      <xsl:call-template name="td"><xsl:with-param name="v" select="gcm:note"/><xsl:with-param name="n" select="180"/></xsl:call-template>
    </tr>
  </xsl:template>

  <xsl:template match="gcm:FunctionalCategory" mode="row">
    <tr>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@category"/></xsl:call-template>
      <td><xsl:value-of select="count(gcm:pathway)"/></td>
      <xsl:call-template name="td"><xsl:with-param name="v" select="gcm:pathway[1]/@name"/><xsl:with-param name="n" select="130"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="gcm:pathway[1]/@ID"/></xsl:call-template>
    </tr>
  </xsl:template>

  <xsl:template match="gcm:publication" mode="row">
    <tr>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@id"/></xsl:call-template>
      <td><xsl:value-of select="@pubmed"/></td>
      <td><xsl:value-of select="@year"/></td>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@doi"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="gcm:title"/><xsl:with-param name="n" select="190"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="gcm:authors[1]"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="gcm:source"/><xsl:with-param name="n" select="110"/></xsl:call-template>
    </tr>
  </xsl:template>

  <xsl:template match="gcm:classTerm" mode="row">
    <tr>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@id"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@name"/><xsl:with-param name="n" select="120"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@parent"/></xsl:call-template>
      <xsl:call-template name="td"><xsl:with-param name="v" select="@abbreviation"/></xsl:call-template>
    </tr>
  </xsl:template>

  <xsl:template match="vck:authors | vck:publications | vck:URLs | vck:string" mode="row">
    <tr>
      <td><xsl:value-of select="local-name()"/></td>
      <xsl:call-template name="td"><xsl:with-param name="v" select="."/><xsl:with-param name="n" select="260"/></xsl:call-template>
    </tr>
  </xsl:template>

  <!-- ================================================================== -->
  <!-- Graphical views (pure inline SVG, deterministic layout)              -->
  <!-- ================================================================== -->

  <!-- Genome track: linear mapping of gene coordinates onto the replicon. -->
  <xsl:template name="genome-tracks">
    <xsl:for-each select="$replicons">
      <xsl:variable name="len" select="number(@length)"/>
      <xsl:if test="$len &gt; 0">
        <xsl:variable name="scale" select="960 div $len"/>
        <div class="figure">
          <h3>基因组基因轨道 · <xsl:value-of select="@genomeName"/></h3>
          <p class="cap">按复制子长度 <xsl:value-of select="format-number($len, '#,##0')"/> bp 线性映射基因坐标，正链与反链分轨着色；仅绘制前 80 个基因，鼠标悬停可查看基因名。</p>
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1000 190" preserveAspectRatio="xMidYMid meet" role="img">
            <rect x="0" y="0" width="1000" height="190" rx="12" fill="#0d1727"/>
            <rect x="20" y="60" width="960" height="70" rx="6" fill="#111c2e" stroke="#1b2a41"/>
            <line x1="20" y1="95" x2="980" y2="95" stroke="#334155" stroke-width="1"/>
            <line x1="20" y1="56" x2="20" y2="134" stroke="#334155" stroke-width="1"/>
            <line x1="260" y1="56" x2="260" y2="134" stroke="#232f42" stroke-width="1"/>
            <line x1="500" y1="56" x2="500" y2="134" stroke="#232f42" stroke-width="1"/>
            <line x1="740" y1="56" x2="740" y2="134" stroke="#232f42" stroke-width="1"/>
            <line x1="980" y1="56" x2="980" y2="134" stroke="#334155" stroke-width="1"/>
            <text x="20" y="152" fill="#94a3b8" font-size="10" text-anchor="middle">0</text>
            <text x="260" y="152" fill="#94a3b8" font-size="10" text-anchor="middle"><xsl:value-of select="format-number($len div 4, '#,##0')"/></text>
            <text x="500" y="152" fill="#94a3b8" font-size="10" text-anchor="middle"><xsl:value-of select="format-number($len div 2, '#,##0')"/></text>
            <text x="740" y="152" fill="#94a3b8" font-size="10" text-anchor="middle"><xsl:value-of select="format-number($len * 3 div 4, '#,##0')"/></text>
            <text x="980" y="152" fill="#94a3b8" font-size="10" text-anchor="middle"><xsl:value-of select="format-number($len, '#,##0')"/></text>
            <text x="500" y="178" fill="#64748b" font-size="10" text-anchor="middle">genomic position (bp)</text>
            <xsl:for-each select="gcm:operons/gcm:TranscriptUnit/gcm:genes[position() &lt;= 80]">
              <xsl:variable name="x0" select="20 + number(@left) * $scale"/>
              <xsl:variable name="w0" select="(number(@right) - number(@left)) * $scale"/>
              <xsl:variable name="w">
                <xsl:choose>
                  <xsl:when test="$w0 &lt; 2">2</xsl:when>
                  <xsl:when test="$w0 &gt; 960">960</xsl:when>
                  <xsl:otherwise><xsl:value-of select="$w0"/></xsl:otherwise>
                </xsl:choose>
              </xsl:variable>
              <xsl:variable name="gy">
                <xsl:choose>
                  <xsl:when test="@strand='Forward'">66</xsl:when>
                  <xsl:when test="@strand='Reverse'">108</xsl:when>
                  <xsl:otherwise>88</xsl:otherwise>
                </xsl:choose>
              </xsl:variable>
              <xsl:variable name="gc">
                <xsl:choose>
                  <xsl:when test="@strand='Forward'">#14b8a6</xsl:when>
                  <xsl:when test="@strand='Reverse'">#7c3aed</xsl:when>
                  <xsl:otherwise>#2563eb</xsl:otherwise>
                </xsl:choose>
              </xsl:variable>
              <rect height="16" rx="2" opacity="0.88">
                <xsl:attribute name="x"><xsl:value-of select="format-number($x0, '0.00')"/></xsl:attribute>
                <xsl:attribute name="y"><xsl:value-of select="$gy"/></xsl:attribute>
                <xsl:attribute name="width"><xsl:value-of select="format-number(number($w), '0.00')"/></xsl:attribute>
                <xsl:attribute name="fill"><xsl:value-of select="$gc"/></xsl:attribute>
                <title><xsl:value-of select="concat(@name, ' [', @locus_tag, '] ', @left, ' - ', @right, ' ', @strand)"/></title>
              </rect>
            </xsl:for-each>
          </svg>
          <div class="legend">
            <span><i style="background:#14b8a6"><xsl:text> </xsl:text></i>正链 Forward</span>
            <span><i style="background:#7c3aed"><xsl:text> </xsl:text></i>反链 Reverse</span>
            <span><i style="background:#2563eb"><xsl:text> </xsl:text></i>未标注链</span>
            <span>共 <xsl:value-of select="format-number(count(gcm:operons/gcm:TranscriptUnit/gcm:genes), '#,##0')"/> 个基因</span>
          </div>
        </div>
      </xsl:if>
    </xsl:for-each>
  </xsl:template>

  <!-- One bar of the composition chart. Wrapped in g to keep SVG namespace. -->
  <xsl:template name="bar">
    <xsl:param name="label"/>
    <xsl:param name="value"/>
    <xsl:param name="max"/>
    <xsl:param name="y"/>
    <xsl:variable name="safeMax">
      <xsl:choose>
        <xsl:when test="number($max) = 0">1</xsl:when>
        <xsl:otherwise><xsl:value-of select="number($max)"/></xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="w" select="600 * number($value) div number($safeMax)"/>
    <g xmlns="http://www.w3.org/2000/svg">
      <text x="10" y="{$y + 15}" fill="#94a3b8" font-size="11"><xsl:value-of select="$label"/></text>
      <rect x="190" y="{$y}" width="600" height="18" rx="4" fill="#1b2a41"/>
      <rect x="190" y="{$y}" width="{$w}" height="18" rx="4" fill="url(#barGrad)"/>
      <text x="800" y="{$y + 14}" fill="#e6edf3" font-size="11"><xsl:value-of select="format-number(number($value), '#,##0')"/></text>
    </g>
  </xsl:template>

  <!-- Section size comparison. -->
  <xsl:template name="composition-chart">
    <xsl:variable name="maxRaw">
      <xsl:call-template name="max-of">
        <xsl:with-param name="nums" select="concat(count($classTerms), ' ', count($compounds), ' ', count($genes), ' ', count($proteins), ' ', count($regulations), ' ', count($tus), ' ', $reactionTotal, ' ', count($enzymes), ' ', count($pathways), ' ', count($publications), ' ', count($promoters), ' ', count($bindingSites))"/>
      </xsl:call-template>
    </xsl:variable>
    <xsl:variable name="max" select="number($maxRaw)"/>
    <div class="figure">
      <h3>分区记录数对比</h3>
      <p class="cap">各分区条目数量（按模型中的实际节点计数，线性归一化到最大分区）。</p>
      <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 860 415" preserveAspectRatio="xMidYMid meet" role="img">
        <defs>
          <linearGradient id="barGrad" x1="0" y1="0" x2="1" y2="0">
            <stop offset="0%" stop-color="#14b8a6"/>
            <stop offset="100%" stop-color="#2563eb"/>
          </linearGradient>
        </defs>
        <xsl:call-template name="bar"><xsl:with-param name="label" select="'分类术语'"/><xsl:with-param name="value" select="count($classTerms)"/><xsl:with-param name="max" select="$max"/><xsl:with-param name="y" select="8"/></xsl:call-template>
        <xsl:call-template name="bar"><xsl:with-param name="label" select="'化合物'"/><xsl:with-param name="value" select="count($compounds)"/><xsl:with-param name="max" select="$max"/><xsl:with-param name="y" select="42"/></xsl:call-template>
        <xsl:call-template name="bar"><xsl:with-param name="label" select="'基因'"/><xsl:with-param name="value" select="count($genes)"/><xsl:with-param name="max" select="$max"/><xsl:with-param name="y" select="76"/></xsl:call-template>
        <xsl:call-template name="bar"><xsl:with-param name="label" select="'蛋白质'"/><xsl:with-param name="value" select="count($proteins)"/><xsl:with-param name="max" select="$max"/><xsl:with-param name="y" select="110"/></xsl:call-template>
        <xsl:call-template name="bar"><xsl:with-param name="label" select="'转录调控'"/><xsl:with-param name="value" select="count($regulations)"/><xsl:with-param name="max" select="$max"/><xsl:with-param name="y" select="144"/></xsl:call-template>
        <xsl:call-template name="bar"><xsl:with-param name="label" select="'转录单元'"/><xsl:with-param name="value" select="count($tus)"/><xsl:with-param name="max" select="$max"/><xsl:with-param name="y" select="178"/></xsl:call-template>
        <xsl:call-template name="bar"><xsl:with-param name="label" select="'代谢反应'"/><xsl:with-param name="value" select="$reactionTotal"/><xsl:with-param name="max" select="$max"/><xsl:with-param name="y" select="212"/></xsl:call-template>
        <xsl:call-template name="bar"><xsl:with-param name="label" select="'启动子'"/><xsl:with-param name="value" select="count($promoters)"/><xsl:with-param name="max" select="$max"/><xsl:with-param name="y" select="246"/></xsl:call-template>
        <xsl:call-template name="bar"><xsl:with-param name="label" select="'结合位点'"/><xsl:with-param name="value" select="count($bindingSites)"/><xsl:with-param name="max" select="$max"/><xsl:with-param name="y" select="280"/></xsl:call-template>
        <xsl:call-template name="bar"><xsl:with-param name="label" select="'酶'"/><xsl:with-param name="value" select="count($enzymes)"/><xsl:with-param name="max" select="$max"/><xsl:with-param name="y" select="314"/></xsl:call-template>
        <xsl:call-template name="bar"><xsl:with-param name="label" select="'文献证据'"/><xsl:with-param name="value" select="count($publications)"/><xsl:with-param name="max" select="$max"/><xsl:with-param name="y" select="348"/></xsl:call-template>
        <xsl:call-template name="bar"><xsl:with-param name="label" select="'代谢通路'"/><xsl:with-param name="value" select="count($pathways)"/><xsl:with-param name="max" select="$max"/><xsl:with-param name="y" select="382"/></xsl:call-template>
      </svg>
    </div>
  </xsl:template>

  <!-- Top compound classes, Muenchian grouping on @classId. -->
  <xsl:template name="class-chart">
    <div class="figure">
      <h3>化合物分类 Top 10</h3>
      <p class="cap">按 classId 分组，条长表示该分类占全部化合物的比例。</p>
      <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 860 360" preserveAspectRatio="xMidYMid meet" role="img">
        <xsl:for-each select="$compounds[generate-id() = generate-id(key('k-compound-class', string(@classId))[1])]">
          <xsl:sort select="count(key('k-compound-class', string(@classId)))" data-type="number" order="descending"/>
          <xsl:if test="position() &lt;= 10">
            <xsl:variable name="c" select="count(key('k-compound-class', string(@classId)))"/>
            <xsl:variable name="y" select="10 + (position() - 1) * 34"/>
            <xsl:variable name="w" select="600 * $c div (count($compounds) + 1)"/>
            <text x="10" y="{$y + 15}" fill="#94a3b8" font-size="11"><xsl:value-of select="substring(string(@classId), 1, 28)"/></text>
            <rect x="240" y="{$y}" width="600" height="18" rx="4" fill="#1b2a41"/>
            <rect x="240" y="{$y}" width="{$w}" height="18" rx="4" fill="#7c3aed" opacity="0.85"/>
            <text x="845" y="{$y + 14}" fill="#e6edf3" font-size="11" text-anchor="end"><xsl:value-of select="format-number($c, '#,##0')"/></text>
          </xsl:if>
        </xsl:for-each>
      </svg>
    </div>
  </xsl:template>

  <!-- Publication years, Muenchian grouping on @year. -->
  <xsl:template name="year-chart">
    <div class="figure">
      <h3>文献证据年份分布 Top 10</h3>
      <p class="cap">按出版年份分组，条长表示该年份文献占全部文献的比例。</p>
      <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 860 360" preserveAspectRatio="xMidYMid meet" role="img">
        <xsl:for-each select="$publications[generate-id() = generate-id(key('k-publication-year', string(@year))[1])]">
          <xsl:sort select="string(@year)" data-type="text" order="descending"/>
          <xsl:if test="position() &lt;= 10">
            <xsl:variable name="c" select="count(key('k-publication-year', string(@year)))"/>
            <xsl:variable name="y" select="10 + (position() - 1) * 34"/>
            <xsl:variable name="w" select="600 * $c div (count($publications) + 1)"/>
            <text x="10" y="{$y + 15}" fill="#94a3b8" font-size="11"><xsl:value-of select="string(@year)"/></text>
            <rect x="240" y="{$y}" width="600" height="18" rx="4" fill="#1b2a41"/>
            <rect x="240" y="{$y}" width="{$w}" height="18" rx="4" fill="#f59e0b" opacity="0.85"/>
            <text x="845" y="{$y + 14}" fill="#e6edf3" font-size="11" text-anchor="end"><xsl:value-of select="format-number($c, '#,##0')"/></text>
          </xsl:if>
        </xsl:for-each>
      </svg>
    </div>
  </xsl:template>

  <!-- Pathway functional categories. -->
  <xsl:template name="category-chart">
    <div class="figure">
      <h3>通路功能类别 Top 10</h3>
      <p class="cap">按 FunctionalCategory 下的通路数量排序，条长表示该类别占全部通路的比例。</p>
      <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 860 360" preserveAspectRatio="xMidYMid meet" role="img">
        <xsl:for-each select="$categories">
          <xsl:sort select="count(gcm:pathway)" data-type="number" order="descending"/>
          <xsl:if test="position() &lt;= 10">
            <xsl:variable name="c" select="count(gcm:pathway)"/>
            <xsl:variable name="y" select="10 + (position() - 1) * 34"/>
            <xsl:variable name="w" select="600 * $c div (count($pathways) + 1)"/>
            <text x="10" y="{$y + 15}" fill="#94a3b8" font-size="11"><xsl:value-of select="substring(string(@category), 1, 28)"/></text>
            <rect x="240" y="{$y}" width="600" height="18" rx="4" fill="#1b2a41"/>
            <rect x="240" y="{$y}" width="{$w}" height="18" rx="4" fill="#22c55e" opacity="0.8"/>
            <text x="845" y="{$y + 14}" fill="#e6edf3" font-size="11" text-anchor="end"><xsl:value-of select="format-number($c, '#,##0')"/></text>
          </xsl:if>
        </xsl:for-each>
      </svg>
    </div>
  </xsl:template>

  <!-- Substrate - reaction - product bipartite view. -->
  <xsl:template name="metabolic-network">
    <div class="figure">
      <h3>底物 — 反应 — 产物 关系图</h3>
      <p class="cap">取前 12 条酶促反应，每行为一个反应：左侧首个底物、中间反应 ID、右侧首个产物。</p>
      <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1000 460" preserveAspectRatio="xMidYMid meet" role="img">
        <rect x="0" y="0" width="1000" height="460" rx="12" fill="#0d1727"/>
        <text x="20" y="16" fill="#64748b" font-size="10">SUBSTRATE</text>
        <text x="370" y="16" fill="#64748b" font-size="10">REACTION</text>
        <text x="690" y="16" fill="#64748b" font-size="10">PRODUCT</text>
        <xsl:for-each select="$enzymaticReactions[position() &lt;= 12]">
          <xsl:variable name="y" select="24 + (position() - 1) * 34"/>
          <xsl:variable name="sub" select="string(gcm:substrate[1])"/>
          <xsl:variable name="prod" select="string(gcm:product[1])"/>
          <line x1="320" y1="{$y + 11}" x2="370" y2="{$y + 11}" stroke="#14b8a6" stroke-width="1.5" opacity="0.55"/>
          <line x1="630" y1="{$y + 11}" x2="690" y2="{$y + 11}" stroke="#7c3aed" stroke-width="1.5" opacity="0.55"/>
          <rect x="20" y="{$y}" width="300" height="22" rx="5" fill="#10293a" stroke="#1c5060"/>
          <text x="30" y="{$y + 15}" fill="#8ff0e3" font-size="11"><xsl:value-of select="substring($sub, 1, 34)"/></text>
          <rect x="370" y="{$y}" width="260" height="22" rx="5" fill="#1b2440" stroke="#3a3380"/>
          <text x="380" y="{$y + 15}" fill="#cbb4ff" font-size="11"><xsl:value-of select="substring(string(@ID), 1, 30)"/></text>
          <rect x="690" y="{$y}" width="290" height="22" rx="5" fill="#10283a" stroke="#1c5060"/>
          <text x="700" y="{$y + 15}" fill="#a9e2ff" font-size="11"><xsl:value-of select="substring($prod, 1, 32)"/></text>
        </xsl:for-each>
      </svg>
      <div class="legend">
        <span><i style="background:#14b8a6"><xsl:text> </xsl:text></i>底物节点</span>
        <span><i style="background:#7c3aed"><xsl:text> </xsl:text></i>反应节点</span>
        <span><i style="background:#38bdf8"><xsl:text> </xsl:text></i>产物节点</span>
        <span>酶促反应共 <xsl:value-of select="format-number(count($enzymaticReactions), '#,##0')"/> 条</span>
      </div>
    </div>
  </xsl:template>

</xsl:stylesheet>
