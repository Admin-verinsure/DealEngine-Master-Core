<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
    <xsl:output method="xml" indent="yes"/>

    <xsl:template match="/data">

			<html>
				<head>
					<style>
						th { font-weight:bold; text-align:left; }
						
					</style>
				</head>
				<body>
					<h1>Report using data from Dataset</h1>

					<table style="border:1px solid black;width:100%;">
						<tr>
							<th>ID</th>
							<th>Item</th>
							<th>Quantity</th>
							<th>Price</th>
						</tr>
						<xsl:for-each select="order_items">
							<tr>
								<td>
									<xsl:value-of select="id"/>
								</td>
								<td>
									<xsl:value-of select="title"/>
								</td>
								<td>
									<xsl:value-of select="quantity"/>
								</td>
								<td>
									$<xsl:value-of select="price"/>
								</td>								
							</tr>
							
						</xsl:for-each>
					</table>
					
				</body>
				
			</html>
			
    </xsl:template>
</xsl:stylesheet>
