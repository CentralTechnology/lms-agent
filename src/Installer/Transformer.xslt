<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl" xmlns:wix="http://schemas.microsoft.com/wix/2006/wi">
  <xsl:output method="xml" indent="yes" />
  <xsl:strip-space elements="*"/>

  <xsl:variable name="CompanyName">Central Technology Ltd</xsl:variable>
  <xsl:variable name="ProductName">License Monitoring System</xsl:variable>
  <xsl:variable name="UpgradeCode">ADAC7706-188B-42E7-922B-50786779042A</xsl:variable>

  <xsl:variable name="Component1">LicenseMonitoringSystem</xsl:variable>
  <xsl:variable name="Component1_Title">License Monitoring System</xsl:variable>
  <xsl:variable name="Component1_ServiceExe">LMS.exe</xsl:variable>
  <xsl:variable name="Component1_ServiceName">License Monitoring System</xsl:variable>
  <xsl:variable name="Component1_ServiceDescr">Automated license reporting solution.</xsl:variable>

  <xsl:template match='/'>
    <Wix xmlns="http://schemas.microsoft.com/wix/2006/wi" xmlns:util="http://schemas.microsoft.com/wix/UtilExtension">

      <Product Id="*" Name="{$ProductName}" Language="1033" Version="1.0.0.0" Manufacturer="{$CompanyName}" UpgradeCode="{$UpgradeCode}">
        <Package InstallerVersion="200" Compressed="yes" InstallScope="perMachine" InstallPrivileges="elevated"/>

        <MajorUpgrade DowngradeErrorMessage="A newer version of {$ProductName} is already installed." />
        <MediaTemplate EmbedCab="yes" />

        <Feature Id="{$Component1}" Title="{$Component1_Title}" Level="1">
          <ComponentGroupRef Id="{$Component1}_ServiceComponents" />
        </Feature>
      </Product>

      <Fragment>
        <Directory Id="TARGETDIR" Name="SourceDir">
          <Directory Id="ProgramFilesFolder">
                <Directory Id="{$Component1}_INSTALLFOLDER" Name="{$ProductName}">
              </Directory>
          </Directory>
        </Directory>
      </Fragment>

      <Fragment>
        <ComponentGroup Id="{$Component1}_ServiceComponents">
          <Component Id="{$Component1}_ServiceComponent" Directory="{$Component1}_INSTALLFOLDER" Guid="2158608F-0EA8-4AD0-9CE9-BBF95B20663F">
            <CreateFolder/>
            <xsl:call-template name="Component1_ReferencesTemplate" />           
            <ServiceInstall
  Id="LMSServiceInstaller"
  Type="ownProcess"
  Vital="yes"
  Name="{$Component1_ServiceName}"
  DisplayName="{$Component1_ServiceName}"
  Description="{$Component1_ServiceDescr}"
  Start="auto"
  Account="LocalSystem"
  ErrorControl="ignore"
  Interactive="no"
						>
              <!--<ServiceDependency Id="[DependencyServiceName]"/>-->
              <!--<util:PermissionEx
									User="Authenticated Users"
									GenericAll="yes"
									ServiceChangeConfig="yes"
									ServiceEnumerateDependents="yes"
									ChangePermission="yes"
									ServiceInterrogate="yes"
									ServicePauseContinue="yes"
									ServiceQueryConfig="yes"
									ServiceQueryStatus="yes"
									ServiceStart="yes"
									ServiceStop="yes" />-->
            </ServiceInstall>
            <ServiceControl Id="{$Component1}StartService" Start="install" Name="{$Component1_ServiceName}" Wait="yes" />
            <ServiceControl Id="{$Component1}StopService" Stop="both" Remove="uninstall" Name="{$Component1_ServiceName}" Wait="yes" />
          </Component>

        </ComponentGroup>

      </Fragment>

    </Wix>
  </xsl:template>

  <xsl:template name="Component1_ReferencesTemplate" match="@*|node()">
    <xsl:copy>
      <xsl:for-each select="wix:Wix/wix:Fragment/wix:ComponentGroup/wix:Component/wix:File[@Source and not (contains(@Source, '.pdb')) 
                    and not (contains(@Source, '.vshost.')) 
                    and not (contains(@Source, '.xml')) 
                    and not (contains(@Source, '.log'))
                    and not (contains(@Source, 'de'))
                    and not (contains(@Source, 'es'))
                    and not (contains(@Source, 'fr'))
                    and not (contains(@Source, 'it'))
                    and not (contains(@Source, 'ja'))
                    and not (contains(@Source, 'ko'))
                    and not (contains(@Source, 'logs'))
                    and not (contains(@Source, 'ru'))
                    and not (contains(@Source, 'za-Hans'))
                    and not (contains(@Source, 'zh-Hant'))]">
        <xsl:apply-templates select="."/>
      </xsl:for-each>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="wix:Wix/wix:Fragment/wix:ComponentGroup/wix:Component/wix:File">
    <xsl:copy>
      <xsl:choose>
        <xsl:when test="not (contains(@Source, 'LMS.exe')) or (contains(@Source, '.config'))">
          <xsl:apply-templates select="@*[name()!='KeyPath']"/>
          <xsl:attribute name="Vital">
            <xsl:value-of select="'yes'"/>
          </xsl:attribute>
        </xsl:when>
        <xsl:otherwise>
          <xsl:apply-templates select="@*"/>
          <xsl:attribute name="Vital">
            <xsl:value-of select="'yes'"/>
          </xsl:attribute>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:copy>
  </xsl:template>

</xsl:stylesheet>