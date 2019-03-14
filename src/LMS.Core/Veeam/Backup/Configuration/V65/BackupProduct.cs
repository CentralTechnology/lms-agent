using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Core.Veeam.Backup.Configuration.V80;
using Microsoft.Win32;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
  public class BackupProduct : IProduct
  {
    public static readonly Guid Guid = new Guid("{B1E61D9B-8D78-4419-8F63-D21279F71A56}");
    public const string Id = "{B1E61D9B-8D78-4419-8F63-D21279F71A56}";
    public const string RegistryKey = "SOFTWARE\\Veeam\\Veeam Backup and Replication";
    private readonly Lazy<Version> _assemblyVersionLazy;
    private readonly Lazy<Version> _productVersionLazy;
    private readonly Lazy<ProductReleaseAttributes> _productReleaseDataLazy;
    private readonly Lazy<IProductProcesses> _productProcessesLazy;
    private readonly Lazy<ProductServers> _productServersLazy;
    private readonly Lazy<bool> _isPreviewLazy;

    private BackupProduct()
    {
      this._assemblyVersionLazy = new Lazy<Version>((Func<Version>) (() => VersionExtractor.AssemblyVersion));
      this._productVersionLazy = new Lazy<Version>((Func<Version>) (() => VersionExtractor.ProductVersion));
      this._productReleaseDataLazy = new Lazy<ProductReleaseAttributes>((Func<ProductReleaseAttributes>) (() => ProductReleases.Load(@"&lt;?xml version=""1.0"" encoding=""utf-8""?&gt;
&lt;product upgradable=""9.0.0.1715"" preview=""false"" xmlns=""http://tempuri.org/ProductReleases.xsd""&gt;
  &lt;!-- 6.0 --&gt;
  &lt;release version=""6.0.0.153"" update=""0"" database=""455"" supported=""false"" name=""Veeam Backup and Replication 6.0""/&gt;
  &lt;release version=""6.0.0.158"" update=""1"" database=""456"" supported=""false"" name=""Veeam Backup and Replication 6.0 Patch 1""/&gt;
  &lt;release version=""6.0.0.164"" update=""2"" database=""457"" supported=""false"" name=""Veeam Backup and Replication 6.0 Patch 2""/&gt;
  &lt;release version=""6.0.0.181"" update=""3"" database=""458"" supported=""false"" name=""Veeam Backup and Replication 6.0 Patch 3""/&gt;
  &lt;release version=""6.0.0.201"" update=""3"" database=""459"" supported=""false"" name=""Veeam Backup and Replication 6.0 Patch 3""/&gt;
  
  &lt;!-- 6.1 --&gt;
  &lt;release version=""6.1.0.181"" update=""0"" database=""520"" supported=""false"" name=""Veeam Backup and Replication 6.1""/&gt;
  &lt;release version=""6.1.0.203"" update=""1"" database=""523"" supported=""false"" name=""Veeam Backup and Replication 6.1 Patch 1""/&gt;
  &lt;release version=""6.1.0.204"" update=""1"" database=""523"" supported=""false"" name=""Veeam Backup and Replication 6.1 Patch 1""/&gt;
  &lt;release version=""6.1.0.205"" update=""1"" database=""523"" supported=""false"" name=""Veeam Backup and Replication 6.1 Patch 1.1""/&gt;
  &lt;release version=""6.1.0.207"" update=""2"" database=""523"" supported=""false"" name=""Veeam Backup and Replication 6.1 Patch 2""/&gt;
  
  &lt;!-- 6.5 --&gt;
  &lt;release version=""6.5.0.106"" update=""0"" database=""633"" supported=""true"" name=""Veeam Backup and Replication 6.5""/&gt;
  &lt;release version=""6.5.0.109"" update=""0"" database=""633"" supported=""true"" name=""Veeam Backup and Replication 6.5""/&gt;
  &lt;release version=""6.5.0.128"" update=""1"" database=""634"" supported=""true"" name=""Veeam Backup and Replication 6.5 Patch 1""/&gt;
  &lt;release version=""6.5.0.133"" update=""2"" database=""634"" supported=""true"" name=""Veeam Backup and Replication 6.5 Patch 2""/&gt;
  &lt;release version=""6.5.0.144"" update=""3"" database=""638"" supported=""true"" name=""Veeam Backup and Replication 6.5 Patch 3""/&gt;
  
  &lt;!-- 7.0 --&gt;
  &lt;release version=""7.0.0.521"" update=""0"" database=""1094"" supported=""false"" name=""Veeam Backup and Replication 7.0 Beta 1""/&gt;
  &lt;release version=""7.0.0.663"" update=""0"" database=""1167"" supported=""false"" name=""Veeam Backup and Replication 7.0 Beta 2""/&gt;
  &lt;release version=""7.0.0.690"" update=""0"" database=""1179"" supported=""true""  name=""Veeam Backup and Replication 7.0""/&gt;
  &lt;release version=""7.0.0.715"" update=""1"" database=""1181"" supported=""true""  name=""Veeam Backup and Replication 7.0 Patch 1""/&gt;
  &lt;release version=""7.0.0.764"" update=""2"" database=""1196"" supported=""true""  name=""Veeam Backup and Replication 7.0 Patch 2""/&gt;
  &lt;release version=""7.0.0.771"" update=""2"" database=""1196"" supported=""true""  name=""Veeam Backup and Replication 7.0 Patch 2""/&gt;
  &lt;release version=""7.0.0.833"" update=""3"" database=""1199"" supported=""true""  name=""Veeam Backup and Replication 7.0 Patch 3""/&gt;
  &lt;release version=""7.0.0.839"" update=""3"" database=""1199"" supported=""true""  name=""Veeam Backup and Replication 7.0 Patch 3""/&gt;
  &lt;release version=""7.0.0.870"" update=""4"" database=""1205"" supported=""true""  name=""Veeam Backup and Replication 7.0 Patch 4""/&gt;
  &lt;release version=""7.0.0.871"" update=""4"" database=""1205"" supported=""true""  name=""Veeam Backup and Replication 7.0 Patch 4""/&gt;

  &lt;!-- 8.0 --&gt;
  &lt;release version=""8.0.0.266""  update=""0"" database=""1563"" supported=""false"" name=""Veeam Backup and Replication 8.0 Preview 1""/&gt;
  &lt;release version=""8.0.0.267""  update=""0"" database=""1563"" supported=""false"" name=""Veeam Backup and Replication 8.0 Preview 1""/&gt;
  &lt;release version=""8.0.0.427""  update=""0"" database=""1656"" supported=""false"" name=""Veeam Backup and Replication 8.0 Beta 1""/&gt;
  &lt;release version=""8.0.0.592""  update=""0"" database=""1745"" supported=""false"" name=""Veeam Backup and Replication 8.0 Beta 2""/&gt;
  &lt;release version=""8.0.0.754""  update=""0"" database=""1858"" supported=""false"" name=""Veeam Backup and Replication 8.0 Beta 3""/&gt;
  &lt;release version=""8.0.0.807""  update=""0"" database=""1870"" supported=""true""  name=""Veeam Backup and Replication 8.0""/&gt;
  &lt;release version=""8.0.0.817""  update=""0"" database=""1870"" supported=""true""  name=""Veeam Backup and Replication 8.0""/&gt;
  &lt;release version=""8.0.0.831""  update=""0"" database=""1870"" supported=""true""  name=""Veeam Backup and Replication 8.0""/&gt;
  &lt;release version=""8.0.0.917""  update=""1"" database=""1872"" supported=""true""  name=""Veeam Backup and Replication 8.0 Patch 1""/&gt;
  &lt;release version=""8.0.0.1274"" update=""2"" database=""1991"" supported=""false"" name=""Veeam Backup and Replication 8.0 Update 2""/&gt;
  &lt;release version=""8.0.0.2018"" update=""2"" database=""2016"" supported=""true""  name=""Veeam Backup and Replication 8.0 Update 2""/&gt;
  &lt;release version=""8.0.0.2021"" update=""2"" database=""2017"" supported=""true""  name=""Veeam Backup and Replication 8.0 Update 2""/&gt;
  &lt;release version=""8.0.0.2029"" update=""2"" database=""2018"" supported=""true""  name=""Veeam Backup and Replication 8.0 Update 2""/&gt;
  &lt;release version=""8.0.0.2030"" update=""2"" database=""2018"" supported=""true""  name=""Veeam Backup and Replication 8.0 Update 2""/&gt;
  &lt;release version=""8.0.0.2084"" update=""3"" database=""2022"" supported=""true""  name=""Veeam Backup and Replication 8.0 Update 3""/&gt;

  &lt;!-- 9.0 --&gt;
  &lt;release version=""9.0.0.285""  update=""0"" database=""2351"" supported=""false"" name=""Veeam Backup and Replication 9.0 Preview 1""/&gt;
  &lt;release version=""9.0.0.293""  update=""0"" database=""2351"" supported=""false"" name=""Veeam Backup and Replication 9.0 Preview 2""/&gt;
  &lt;release version=""9.0.0.557""  update=""0"" database=""2649"" supported=""false"" name=""Veeam Backup and Replication 9.0 Beta 1""/&gt;
  &lt;release version=""9.0.0.560""  update=""0"" database=""2649"" supported=""false"" name=""Veeam Backup and Replication 9.0 Beta 1""/&gt;
  &lt;release version=""9.0.0.773""  update=""0"" database=""2750"" supported=""true""  name=""Veeam Backup and Replication 9.0""/&gt;
  &lt;release version=""9.0.0.902""  update=""0"" database=""2754"" supported=""true""  name=""Veeam Backup and Replication 9.0""/&gt;
  &lt;release version=""9.0.0.1483"" update=""1"" database=""2773"" supported=""false"" name=""Veeam Backup and Replication 9.0 Update 1""/&gt;
  &lt;release version=""9.0.0.1491"" update=""1"" database=""2773"" supported=""true""  name=""Veeam Backup and Replication 9.0 Update 1""/&gt;
  &lt;release version=""9.0.0.1715"" update=""2"" database=""2791"" supported=""true""  name=""Veeam Backup and Replication 9.0 Update 2""/&gt;

  &lt;!-- 9.5 --&gt;
  &lt;release version=""9.5.0.221""  update=""0"" database=""3232"" supported=""false"" name=""Veeam Backup and Replication 9.5 Beta 1""/&gt;
  &lt;release version=""9.5.0.348""  update=""0"" database=""3379"" supported=""false"" name=""Veeam Backup and Replication 9.5 Beta 2""/&gt;
  &lt;release version=""9.5.0.580""  update=""0"" database=""3530"" supported=""true""  name=""Veeam Backup and Replication 9.5""/&gt;
  &lt;release version=""9.5.0.711""  update=""0"" database=""3539"" supported=""true""  name=""Veeam Backup and Replication 9.5""/&gt;
  &lt;release version=""9.5.0.802""  update=""1"" database=""3627"" supported=""true""  name=""Veeam Backup and Replication 9.5 Update 1""/&gt;
  &lt;release version=""9.5.0.823""  update=""1"" database=""3628"" supported=""true""  name=""Veeam Backup and Replication 9.5 Update 1""/&gt;
  &lt;release version=""9.5.0.1038"" update=""2"" database=""3701"" supported=""true""  name=""Veeam Backup and Replication 9.5 Update 2""/&gt;
  &lt;release version=""9.5.0.1046"" update=""2"" database=""3701"" supported=""true""  name=""Veeam Backup and Replication 9.5 Update 2""/&gt;
  &lt;release version=""9.5.0.1335"" update=""3"" database=""4033"" supported=""true""  name=""Veeam Backup and Replication 9.5 Update 3""/&gt;
  &lt;release version=""9.5.0.1536"" update=""3"" database=""4051"" supported=""true""  name=""Veeam Backup and Replication 9.5 Update 3""/&gt;
  &lt;release version=""9.5.0.1922"" update=""4"" database=""4087"" supported=""true""  name=""Veeam Backup and Replication 9.5 Update 3a""/&gt;

  &lt;!-- Current --&gt;
  &lt;release version=""9.5.4.2176"" update=""5"" database=""5006"" supported=""false"" name=""Veeam Backup and Replication 9.5 Update 4 Preview 1""/&gt;
  &lt;release version=""9.5.4.2282"" update=""5"" database=""5170"" supported=""false"" name=""Veeam Backup and Replication 9.5 Update 4 Beta 1""/&gt;
  &lt;release version=""9.5.4.2399"" update=""5"" database=""5349"" supported=""true""  name=""Veeam Backup and Replication 9.5 Update 4""/&gt;
  &lt;release version=""0.0.0.0""    update=""5"" database=""0""    supported=""true""  name=""Veeam Backup and Replication 9.5 Update 4""/&gt;
&lt;/product&gt;")));
      this._productProcessesLazy = new Lazy<IProductProcesses>((Func<IProductProcesses>) (() => ProductProcesses.Load(@"&lt;?xml version=""1.0"" encoding=""utf-8""?&gt;
&lt;product upgradable=""9.0.0.1715"" preview=""false"" xmlns=""http://tempuri.org/ProductReleases.xsd""&gt;
  &lt;!-- 6.0 --&gt;
  &lt;release version=""6.0.0.153"" update=""0"" database=""455"" supported=""false"" name=""Veeam Backup and Replication 6.0""/&gt;
  &lt;release version=""6.0.0.158"" update=""1"" database=""456"" supported=""false"" name=""Veeam Backup and Replication 6.0 Patch 1""/&gt;
  &lt;release version=""6.0.0.164"" update=""2"" database=""457"" supported=""false"" name=""Veeam Backup and Replication 6.0 Patch 2""/&gt;
  &lt;release version=""6.0.0.181"" update=""3"" database=""458"" supported=""false"" name=""Veeam Backup and Replication 6.0 Patch 3""/&gt;
  &lt;release version=""6.0.0.201"" update=""3"" database=""459"" supported=""false"" name=""Veeam Backup and Replication 6.0 Patch 3""/&gt;
  
  &lt;!-- 6.1 --&gt;
  &lt;release version=""6.1.0.181"" update=""0"" database=""520"" supported=""false"" name=""Veeam Backup and Replication 6.1""/&gt;
  &lt;release version=""6.1.0.203"" update=""1"" database=""523"" supported=""false"" name=""Veeam Backup and Replication 6.1 Patch 1""/&gt;
  &lt;release version=""6.1.0.204"" update=""1"" database=""523"" supported=""false"" name=""Veeam Backup and Replication 6.1 Patch 1""/&gt;
  &lt;release version=""6.1.0.205"" update=""1"" database=""523"" supported=""false"" name=""Veeam Backup and Replication 6.1 Patch 1.1""/&gt;
  &lt;release version=""6.1.0.207"" update=""2"" database=""523"" supported=""false"" name=""Veeam Backup and Replication 6.1 Patch 2""/&gt;
  
  &lt;!-- 6.5 --&gt;
  &lt;release version=""6.5.0.106"" update=""0"" database=""633"" supported=""true"" name=""Veeam Backup and Replication 6.5""/&gt;
  &lt;release version=""6.5.0.109"" update=""0"" database=""633"" supported=""true"" name=""Veeam Backup and Replication 6.5""/&gt;
  &lt;release version=""6.5.0.128"" update=""1"" database=""634"" supported=""true"" name=""Veeam Backup and Replication 6.5 Patch 1""/&gt;
  &lt;release version=""6.5.0.133"" update=""2"" database=""634"" supported=""true"" name=""Veeam Backup and Replication 6.5 Patch 2""/&gt;
  &lt;release version=""6.5.0.144"" update=""3"" database=""638"" supported=""true"" name=""Veeam Backup and Replication 6.5 Patch 3""/&gt;
  
  &lt;!-- 7.0 --&gt;
  &lt;release version=""7.0.0.521"" update=""0"" database=""1094"" supported=""false"" name=""Veeam Backup and Replication 7.0 Beta 1""/&gt;
  &lt;release version=""7.0.0.663"" update=""0"" database=""1167"" supported=""false"" name=""Veeam Backup and Replication 7.0 Beta 2""/&gt;
  &lt;release version=""7.0.0.690"" update=""0"" database=""1179"" supported=""true""  name=""Veeam Backup and Replication 7.0""/&gt;
  &lt;release version=""7.0.0.715"" update=""1"" database=""1181"" supported=""true""  name=""Veeam Backup and Replication 7.0 Patch 1""/&gt;
  &lt;release version=""7.0.0.764"" update=""2"" database=""1196"" supported=""true""  name=""Veeam Backup and Replication 7.0 Patch 2""/&gt;
  &lt;release version=""7.0.0.771"" update=""2"" database=""1196"" supported=""true""  name=""Veeam Backup and Replication 7.0 Patch 2""/&gt;
  &lt;release version=""7.0.0.833"" update=""3"" database=""1199"" supported=""true""  name=""Veeam Backup and Replication 7.0 Patch 3""/&gt;
  &lt;release version=""7.0.0.839"" update=""3"" database=""1199"" supported=""true""  name=""Veeam Backup and Replication 7.0 Patch 3""/&gt;
  &lt;release version=""7.0.0.870"" update=""4"" database=""1205"" supported=""true""  name=""Veeam Backup and Replication 7.0 Patch 4""/&gt;
  &lt;release version=""7.0.0.871"" update=""4"" database=""1205"" supported=""true""  name=""Veeam Backup and Replication 7.0 Patch 4""/&gt;

  &lt;!-- 8.0 --&gt;
  &lt;release version=""8.0.0.266""  update=""0"" database=""1563"" supported=""false"" name=""Veeam Backup and Replication 8.0 Preview 1""/&gt;
  &lt;release version=""8.0.0.267""  update=""0"" database=""1563"" supported=""false"" name=""Veeam Backup and Replication 8.0 Preview 1""/&gt;
  &lt;release version=""8.0.0.427""  update=""0"" database=""1656"" supported=""false"" name=""Veeam Backup and Replication 8.0 Beta 1""/&gt;
  &lt;release version=""8.0.0.592""  update=""0"" database=""1745"" supported=""false"" name=""Veeam Backup and Replication 8.0 Beta 2""/&gt;
  &lt;release version=""8.0.0.754""  update=""0"" database=""1858"" supported=""false"" name=""Veeam Backup and Replication 8.0 Beta 3""/&gt;
  &lt;release version=""8.0.0.807""  update=""0"" database=""1870"" supported=""true""  name=""Veeam Backup and Replication 8.0""/&gt;
  &lt;release version=""8.0.0.817""  update=""0"" database=""1870"" supported=""true""  name=""Veeam Backup and Replication 8.0""/&gt;
  &lt;release version=""8.0.0.831""  update=""0"" database=""1870"" supported=""true""  name=""Veeam Backup and Replication 8.0""/&gt;
  &lt;release version=""8.0.0.917""  update=""1"" database=""1872"" supported=""true""  name=""Veeam Backup and Replication 8.0 Patch 1""/&gt;
  &lt;release version=""8.0.0.1274"" update=""2"" database=""1991"" supported=""false"" name=""Veeam Backup and Replication 8.0 Update 2""/&gt;
  &lt;release version=""8.0.0.2018"" update=""2"" database=""2016"" supported=""true""  name=""Veeam Backup and Replication 8.0 Update 2""/&gt;
  &lt;release version=""8.0.0.2021"" update=""2"" database=""2017"" supported=""true""  name=""Veeam Backup and Replication 8.0 Update 2""/&gt;
  &lt;release version=""8.0.0.2029"" update=""2"" database=""2018"" supported=""true""  name=""Veeam Backup and Replication 8.0 Update 2""/&gt;
  &lt;release version=""8.0.0.2030"" update=""2"" database=""2018"" supported=""true""  name=""Veeam Backup and Replication 8.0 Update 2""/&gt;
  &lt;release version=""8.0.0.2084"" update=""3"" database=""2022"" supported=""true""  name=""Veeam Backup and Replication 8.0 Update 3""/&gt;

  &lt;!-- 9.0 --&gt;
  &lt;release version=""9.0.0.285""  update=""0"" database=""2351"" supported=""false"" name=""Veeam Backup and Replication 9.0 Preview 1""/&gt;
  &lt;release version=""9.0.0.293""  update=""0"" database=""2351"" supported=""false"" name=""Veeam Backup and Replication 9.0 Preview 2""/&gt;
  &lt;release version=""9.0.0.557""  update=""0"" database=""2649"" supported=""false"" name=""Veeam Backup and Replication 9.0 Beta 1""/&gt;
  &lt;release version=""9.0.0.560""  update=""0"" database=""2649"" supported=""false"" name=""Veeam Backup and Replication 9.0 Beta 1""/&gt;
  &lt;release version=""9.0.0.773""  update=""0"" database=""2750"" supported=""true""  name=""Veeam Backup and Replication 9.0""/&gt;
  &lt;release version=""9.0.0.902""  update=""0"" database=""2754"" supported=""true""  name=""Veeam Backup and Replication 9.0""/&gt;
  &lt;release version=""9.0.0.1483"" update=""1"" database=""2773"" supported=""false"" name=""Veeam Backup and Replication 9.0 Update 1""/&gt;
  &lt;release version=""9.0.0.1491"" update=""1"" database=""2773"" supported=""true""  name=""Veeam Backup and Replication 9.0 Update 1""/&gt;
  &lt;release version=""9.0.0.1715"" update=""2"" database=""2791"" supported=""true""  name=""Veeam Backup and Replication 9.0 Update 2""/&gt;

  &lt;!-- 9.5 --&gt;
  &lt;release version=""9.5.0.221""  update=""0"" database=""3232"" supported=""false"" name=""Veeam Backup and Replication 9.5 Beta 1""/&gt;
  &lt;release version=""9.5.0.348""  update=""0"" database=""3379"" supported=""false"" name=""Veeam Backup and Replication 9.5 Beta 2""/&gt;
  &lt;release version=""9.5.0.580""  update=""0"" database=""3530"" supported=""true""  name=""Veeam Backup and Replication 9.5""/&gt;
  &lt;release version=""9.5.0.711""  update=""0"" database=""3539"" supported=""true""  name=""Veeam Backup and Replication 9.5""/&gt;
  &lt;release version=""9.5.0.802""  update=""1"" database=""3627"" supported=""true""  name=""Veeam Backup and Replication 9.5 Update 1""/&gt;
  &lt;release version=""9.5.0.823""  update=""1"" database=""3628"" supported=""true""  name=""Veeam Backup and Replication 9.5 Update 1""/&gt;
  &lt;release version=""9.5.0.1038"" update=""2"" database=""3701"" supported=""true""  name=""Veeam Backup and Replication 9.5 Update 2""/&gt;
  &lt;release version=""9.5.0.1046"" update=""2"" database=""3701"" supported=""true""  name=""Veeam Backup and Replication 9.5 Update 2""/&gt;
  &lt;release version=""9.5.0.1335"" update=""3"" database=""4033"" supported=""true""  name=""Veeam Backup and Replication 9.5 Update 3""/&gt;
  &lt;release version=""9.5.0.1536"" update=""3"" database=""4051"" supported=""true""  name=""Veeam Backup and Replication 9.5 Update 3""/&gt;
  &lt;release version=""9.5.0.1922"" update=""4"" database=""4087"" supported=""true""  name=""Veeam Backup and Replication 9.5 Update 3a""/&gt;

  &lt;!-- Current --&gt;
  &lt;release version=""9.5.4.2176"" update=""5"" database=""5006"" supported=""false"" name=""Veeam Backup and Replication 9.5 Update 4 Preview 1""/&gt;
  &lt;release version=""9.5.4.2282"" update=""5"" database=""5170"" supported=""false"" name=""Veeam Backup and Replication 9.5 Update 4 Beta 1""/&gt;
  &lt;release version=""9.5.4.2399"" update=""5"" database=""5349"" supported=""true""  name=""Veeam Backup and Replication 9.5 Update 4""/&gt;
  &lt;release version=""0.0.0.0""    update=""5"" database=""0""    supported=""true""  name=""Veeam Backup and Replication 9.5 Update 4""/&gt;
&lt;/product&gt;")));
      this._productServersLazy = new Lazy<ProductServers>((Func<ProductServers>) (() => ProductServers.Load(@"&lt;?xml version=""1.0"" encoding=""utf-8""?&gt;
&lt;product minsupportedversion=""10.0.1600.22"" xmlns=""http://tempuri.org/ProductServers.xsd""&gt;

  &lt;!-- http://support.microsoft.com/kb/321185 --&gt;
  &lt;!-- http://sqlserverbuilds.blogspot.ru --&gt;
  
  &lt;!-- Microsoft SQL Server 2005 --&gt;
  &lt;server family=""sql2005"" version=""9.0.1399.6""/&gt;
  &lt;server family=""sql2005"" version=""9.0.2047.0"" servicepack=""1""/&gt;
  &lt;server family=""sql2005"" version=""9.0.3042.0"" servicepack=""2""/&gt;
  &lt;server family=""sql2005"" version=""9.0.4035.0"" servicepack=""3""/&gt;
  &lt;server family=""sql2005"" version=""9.0.5000.0"" servicepack=""4""/&gt;

  &lt;!-- Microsoft SQL Server 2008 --&gt;
  &lt;server family=""sql2008"" version=""10.0.1600.22""/&gt;
  &lt;server family=""sql2008"" version=""10.0.2531.0"" servicepack=""1""/&gt;
  &lt;server family=""sql2008"" version=""10.0.4000.0"" servicepack=""2""/&gt;
  &lt;server family=""sql2008"" version=""10.0.5500.0"" servicepack=""3""/&gt;
  &lt;server family=""sql2008"" version=""10.0.6000.29"" servicepack=""4""/&gt;

  &lt;!-- Microsoft SQL Server 2008 R2--&gt;
  &lt;server family=""sql2008r2"" version=""10.50.1600.1""/&gt;
  &lt;server family=""sql2008r2"" version=""10.50.2500.0"" servicepack=""1""/&gt;
  &lt;server family=""sql2008r2"" version=""10.50.4000.0"" servicepack=""2""/&gt;
  &lt;server family=""sql2008r2"" version=""10.50.6000.34"" servicepack=""3""/&gt;

  &lt;!-- Microsoft SQL Server 2012 --&gt;
  &lt;server family=""sql2012"" version=""11.0.2100.60""/&gt;
  &lt;server family=""sql2012"" version=""11.0.3000.0"" servicepack=""1""/&gt;
  &lt;server family=""sql2012"" version=""11.0.5058.0"" servicepack=""2""/&gt;
  &lt;server family=""sql2012"" version=""11.0.6020.0"" servicepack=""3""/&gt;
  &lt;server family=""sql2012"" version=""11.0.7001.0"" servicepack=""4""/&gt;

  &lt;!-- Microsoft SQL Server 2014 --&gt;
  &lt;server family=""sql2014"" version=""12.0.2000.8""/&gt;
  &lt;server family=""sql2014"" version=""12.0.4100.1"" servicepack=""1""/&gt;
  &lt;server family=""sql2014"" version=""12.0.5000.0"" servicepack=""2""/&gt;

  &lt;!-- Microsoft SQL Server 2016 --&gt;
  &lt;server family=""sql2016"" version=""13.0.1601.5""/&gt;
  &lt;server family=""sql2016"" version=""13.0.4001.0"" servicepack=""1""/&gt;
  &lt;server family=""sql2016"" version=""13.0.5026.0"" servicepack=""2""/&gt;

  &lt;!-- Microsoft SQL Server 2017 --&gt;
  &lt;server family=""sql2017"" version=""14.0.1000.169""/&gt;
&lt;/product&gt;")));
      this._isPreviewLazy = new Lazy<bool>((Func<bool>) (() => ProductMode.IsPreview(this._productReleaseDataLazy.Value, (IProduct) this)));
    }

    Guid IProduct.Id
    {
      get
      {
        return new Guid("{B1E61D9B-8D78-4419-8F63-D21279F71A56}");
      }
    }

    string IProduct.Name
    {
      get
      {
        return "Veeam Backup and Replication";
      }
    }

    string IProduct.Alias
    {
      get
      {
        return "Backup";
      }
    }

    string IProduct.DisplayName
    {
      get
      {
        return "Veeam Backup and Replication";
      }
    }

    string IProduct.RegistryKey
    {
      get
      {
        return "SOFTWARE\\Veeam\\Veeam Backup and Replication";
      }
    }

    Version IProduct.AssemblyVersion
    {
      get
      {
        return this._assemblyVersionLazy.Value;
      }
    }

    Version IProduct.ProductVersion
    {
      get
      {
        return this._productVersionLazy.Value;
      }
    }

    bool IProduct.IsPreview
    {
      get
      {
        return this._isPreviewLazy.Value;
      }
    }

    Version IProduct.UpgradableVersion
    {
      get
      {
        return this._productReleaseDataLazy.Value.Upgdarable;
      }
    }

    Version IProduct.MinSupportedServerVersion
    {
      get
      {
        return this._productServersLazy.Value.MinSupportedVersion;
      }
    }

    IEnumerable<IProductRelease> IProduct.LoadReleases(
      Version productVersion,
      IDatabaseVersion productDatabaseVersion)
    {
      return ProductReleases.Load(BackupProductReleases, productVersion, productDatabaseVersion);
    }

    IEnumerable<IProductSetup> IProduct.LoadSetups(
      Version productVersion,
      Guid productPackageCode)
    {
      return ProductSetups.Load(BackupProductSetups, productVersion, productPackageCode);
    }

    IProductProcesses IProduct.LoadProcesses()
    {
      return this._productProcessesLazy.Value;
    }

    IEnumerable<IProductServer> IProduct.LoadServers()
    {
      return (IEnumerable<IProductServer>) this._productServersLazy.Value.Servers;
    }

    IRegistryConfigurationController IProduct.CreateRegistryController(
      bool writable,
      RegistryView registryView)
    {
      return RegistryConfigurationController.Create("SOFTWARE\\Veeam\\Veeam Backup and Replication", writable, registryView);
    }

    IEnumerable<string> IProduct.DatabaseDeployers
    {
      get
      {
        yield return "Veeam.Backup.Service";
        yield return "Veeam.Backup.Configuration.Restore";
      }
    }

    string IProduct.DatabaseFileName
    {
      get
      {
        return "BackupDatabase.xml";
      }
    }

    string IProduct.ProductFolderName
    {
      get
      {
        return "Backup";
      }
    }

    string IProduct.DatabaseLockId
    {
      get
      {
        return "{91BB166B-E197-48F7-A430-7904E013B30C}";
      }
    }

    EventLogSource IProduct.EventLogSource
    {
      get
      {
        return EventLogSource.VeeamMP;
      }
    }

    IDatabaseConfiguration IProduct.CreateDatabaseConfiguration(
      RegistryView registryView)
    {
      return BackupDatabaseConfiguration.Initialize("SOFTWARE\\Veeam\\Veeam Backup and Replication", registryView);
    }

    IDatabaseConfiguration IProduct.LoadDatabaseConfiguration(
      RegistryView registryView)
    {
      return BackupDatabaseConfiguration.Load("SOFTWARE\\Veeam\\Veeam Backup and Replication", registryView);
    }

    public static IProduct Create()
    {
      return (IProduct) new BackupProduct();
    }

    string IProduct.MainHelpUrl
    {
      get
      {
        return "https://redirect.veeam.com/helpbackup95u4topicid=";
      }
    }

    string IProduct.AdvancedHelpUrl
    {
      get
      {
        return "https://www.veeam.com/helpbackupfree95topicid=";
      }
    }

    private const string BackupProductSetups = @"&lt;?xml version=""1.0"" encoding=""utf-8""?&gt;
&lt;product xmlns=""http://tempuri.org/ProductSetups.xsd""&gt;
  &lt;!-- 6.0 --&gt;
  &lt;setup version=""6.0.0.153"" platform=""win64"" filename=""bu_x64.msi""     packagecode=""{F8CF953C-49FB-4A0B-9623-FDDA1E878CD0}"" productcode=""{7F57DA36-807F-4257-B1F3-CFBCBDF83D14}"" upgradecode=""{546BF212-A131-437F-9582-E30D933C1F4F}""/&gt;
  &lt;setup version=""6.0.0.153"" platform=""win32"" filename=""bu_i386.msi""    packagecode=""{0124C2F9-EA22-4180-AA81-6F1F41355507}"" productcode=""{22856EA4-1FE1-4338-9DF3-F5EA69D2E617}"" upgradecode=""{546BF212-A131-437F-9582-E30D933C1F4F}""/&gt;

  &lt;!-- 6.1 --&gt;
  &lt;setup version=""6.1.0.181"" platform=""win64"" filename=""bu_x64.msi""     packagecode=""{2C581B61-3613-46C3-812F-9AB8F8A6A134}"" productcode=""{D6B844F4-6F3A-4ABA-9A49-1583E1F685E7}"" upgradecode=""{546BF212-A131-437F-9582-E30D933C1F4F}""/&gt;
  &lt;setup version=""6.1.0.181"" platform=""win32"" filename=""bu_i386.msi""    packagecode=""{60AC107B-DFD4-4A2C-A48A-63BA6CB0228F}"" productcode=""{51E38603-5252-4028-B712-D7DEE7811346}"" upgradecode=""{546BF212-A131-437F-9582-E30D933C1F4F}""/&gt;

  &lt;!-- 6.5 --&gt;
  &lt;setup version=""6.5.0.106"" platform=""win64"" filename=""bu_x64.msi""     packagecode=""{C8295F12-5CE0-4BEC-A1C6-B29246E95278}"" productcode=""{19EE6A17-3A82-47C7-B9E4-D82161DB0795}"" upgradecode=""{546BF212-A131-437F-9582-E30D933C1F4F}""/&gt;
  &lt;setup version=""6.5.0.106"" platform=""win32"" filename=""bu_i386.msi""    packagecode=""{E23B7779-9551-4C8B-8D9B-5EC3FDA26560}"" productcode=""{0A289FD0-59AD-413A-97C5-6184B215D711}"" upgradecode=""{546BF212-A131-437F-9582-E30D933C1F4F}""/&gt;

  &lt;!-- 7.0 --&gt;
  &lt;setup version=""7.0.0.690"" platform=""win64"" filename=""bu_x64.msi""     packagecode=""{07158C0B-663A-44DF-82F5-47AA5E740BA8}"" productcode=""{01E4DAAF-48B9-4A33-8C5A-0BDCA5A6CDF4}"" upgradecode=""{546BF212-A131-437F-9582-E30D933C1F4F}""/&gt;

  &lt;!-- 8.0 --&gt;
  &lt;setup version=""8.0.0.817"" platform=""win64"" filename=""bu_x64.msi""     packagecode=""{A9DA3FE1-997F-4956-B43D-6B67610E35E6}"" productcode=""{52EC4366-FF56-4B08-817F-7797C72397A0}"" upgradecode=""{546BF212-A131-437F-9582-E30D933C1F4F}""/&gt;

  &lt;!-- 9.0 --&gt;
  &lt;setup version=""9.0.0.733"" platform=""win64"" filename=""server.x64.msi"" packagecode=""{CFFAE96D-C9CD-4858-8E8D-F1BF7DB45E08}"" productcode=""{34AD3199-9693-49D6-9197-AEA759082EC2}"" upgradecode=""{546BF212-A131-437F-9582-E30D933C1F4F}""/&gt;

  &lt;!-- 9.5 --&gt;
  &lt;setup version=""9.5.0.711"" platform=""win64"" filename=""server.x64.msi"" packagecode=""{B7393253-26A6-48C9-AD0C-C01C696FCCEF}"" productcode=""{1E9C4410-1AAD-4F0F-9DE3-588302E3EE87}"" upgradecode=""{546BF212-A131-437F-9582-E30D933C1F4F}""/&gt;

  &lt;!-- 9.5 U4 --&gt;
  &lt;setup version=""0.0.0.0""   platform=""win64"" filename=""server.x64.msi"" packagecode=""{00000000-0000-0000-0000-000000000000}"" productcode=""{C2011D75-F0EA-4E10-AA66-1B871639C259}"" upgradecode=""{546BF212-A131-437F-9582-E30D933C1F4F}""/&gt;
&lt;/product&gt;";

    private const string BackupProductReleases = @"<?xml version=""1.0"" encoding=""utf-8""?>
<product upgradable=""9.0.0.1715"" preview=""false"" xmlns=""http://tempuri.org/ProductReleases.xsd"">
  <!-- 6.0 -->
  <release version=""6.0.0.153"" update=""0"" database=""455"" supported=""false"" name=""Veeam Backup and Replication 6.0""/>
  <release version=""6.0.0.158"" update=""1"" database=""456"" supported=""false"" name=""Veeam Backup and Replication 6.0 Patch 1""/>
  <release version=""6.0.0.164"" update=""2"" database=""457"" supported=""false"" name=""Veeam Backup and Replication 6.0 Patch 2""/>
  <release version=""6.0.0.181"" update=""3"" database=""458"" supported=""false"" name=""Veeam Backup and Replication 6.0 Patch 3""/>
  <release version=""6.0.0.201"" update=""3"" database=""459"" supported=""false"" name=""Veeam Backup and Replication 6.0 Patch 3""/>
  
  <!-- 6.1 -->
  <release version=""6.1.0.181"" update=""0"" database=""520"" supported=""false"" name=""Veeam Backup and Replication 6.1""/>
  <release version=""6.1.0.203"" update=""1"" database=""523"" supported=""false"" name=""Veeam Backup and Replication 6.1 Patch 1""/>
  <release version=""6.1.0.204"" update=""1"" database=""523"" supported=""false"" name=""Veeam Backup and Replication 6.1 Patch 1""/>
  <release version=""6.1.0.205"" update=""1"" database=""523"" supported=""false"" name=""Veeam Backup and Replication 6.1 Patch 1.1""/>
  <release version=""6.1.0.207"" update=""2"" database=""523"" supported=""false"" name=""Veeam Backup and Replication 6.1 Patch 2""/>
  
  <!-- 6.5 -->
  <release version=""6.5.0.106"" update=""0"" database=""633"" supported=""true"" name=""Veeam Backup and Replication 6.5""/>
  <release version=""6.5.0.109"" update=""0"" database=""633"" supported=""true"" name=""Veeam Backup and Replication 6.5""/>
  <release version=""6.5.0.128"" update=""1"" database=""634"" supported=""true"" name=""Veeam Backup and Replication 6.5 Patch 1""/>
  <release version=""6.5.0.133"" update=""2"" database=""634"" supported=""true"" name=""Veeam Backup and Replication 6.5 Patch 2""/>
  <release version=""6.5.0.144"" update=""3"" database=""638"" supported=""true"" name=""Veeam Backup and Replication 6.5 Patch 3""/>
  
  <!-- 7.0 -->
  <release version=""7.0.0.521"" update=""0"" database=""1094"" supported=""false"" name=""Veeam Backup and Replication 7.0 Beta 1""/>
  <release version=""7.0.0.663"" update=""0"" database=""1167"" supported=""false"" name=""Veeam Backup and Replication 7.0 Beta 2""/>
  <release version=""7.0.0.690"" update=""0"" database=""1179"" supported=""true""  name=""Veeam Backup and Replication 7.0""/>
  <release version=""7.0.0.715"" update=""1"" database=""1181"" supported=""true""  name=""Veeam Backup and Replication 7.0 Patch 1""/>
  <release version=""7.0.0.764"" update=""2"" database=""1196"" supported=""true""  name=""Veeam Backup and Replication 7.0 Patch 2""/>
  <release version=""7.0.0.771"" update=""2"" database=""1196"" supported=""true""  name=""Veeam Backup and Replication 7.0 Patch 2""/>
  <release version=""7.0.0.833"" update=""3"" database=""1199"" supported=""true""  name=""Veeam Backup and Replication 7.0 Patch 3""/>
  <release version=""7.0.0.839"" update=""3"" database=""1199"" supported=""true""  name=""Veeam Backup and Replication 7.0 Patch 3""/>
  <release version=""7.0.0.870"" update=""4"" database=""1205"" supported=""true""  name=""Veeam Backup and Replication 7.0 Patch 4""/>
  <release version=""7.0.0.871"" update=""4"" database=""1205"" supported=""true""  name=""Veeam Backup and Replication 7.0 Patch 4""/>

  <!-- 8.0 -->
  <release version=""8.0.0.266""  update=""0"" database=""1563"" supported=""false"" name=""Veeam Backup and Replication 8.0 Preview 1""/>
  <release version=""8.0.0.267""  update=""0"" database=""1563"" supported=""false"" name=""Veeam Backup and Replication 8.0 Preview 1""/>
  <release version=""8.0.0.427""  update=""0"" database=""1656"" supported=""false"" name=""Veeam Backup and Replication 8.0 Beta 1""/>
  <release version=""8.0.0.592""  update=""0"" database=""1745"" supported=""false"" name=""Veeam Backup and Replication 8.0 Beta 2""/>
  <release version=""8.0.0.754""  update=""0"" database=""1858"" supported=""false"" name=""Veeam Backup and Replication 8.0 Beta 3""/>
  <release version=""8.0.0.807""  update=""0"" database=""1870"" supported=""true""  name=""Veeam Backup and Replication 8.0""/>
  <release version=""8.0.0.817""  update=""0"" database=""1870"" supported=""true""  name=""Veeam Backup and Replication 8.0""/>
  <release version=""8.0.0.831""  update=""0"" database=""1870"" supported=""true""  name=""Veeam Backup and Replication 8.0""/>
  <release version=""8.0.0.917""  update=""1"" database=""1872"" supported=""true""  name=""Veeam Backup and Replication 8.0 Patch 1""/>
  <release version=""8.0.0.1274"" update=""2"" database=""1991"" supported=""false"" name=""Veeam Backup and Replication 8.0 Update 2""/>
  <release version=""8.0.0.2018"" update=""2"" database=""2016"" supported=""true""  name=""Veeam Backup and Replication 8.0 Update 2""/>
  <release version=""8.0.0.2021"" update=""2"" database=""2017"" supported=""true""  name=""Veeam Backup and Replication 8.0 Update 2""/>
  <release version=""8.0.0.2029"" update=""2"" database=""2018"" supported=""true""  name=""Veeam Backup and Replication 8.0 Update 2""/>
  <release version=""8.0.0.2030"" update=""2"" database=""2018"" supported=""true""  name=""Veeam Backup and Replication 8.0 Update 2""/>
  <release version=""8.0.0.2084"" update=""3"" database=""2022"" supported=""true""  name=""Veeam Backup and Replication 8.0 Update 3""/>

  <!-- 9.0 -->
  <release version=""9.0.0.285""  update=""0"" database=""2351"" supported=""false"" name=""Veeam Backup and Replication 9.0 Preview 1""/>
  <release version=""9.0.0.293""  update=""0"" database=""2351"" supported=""false"" name=""Veeam Backup and Replication 9.0 Preview 2""/>
  <release version=""9.0.0.557""  update=""0"" database=""2649"" supported=""false"" name=""Veeam Backup and Replication 9.0 Beta 1""/>
  <release version=""9.0.0.560""  update=""0"" database=""2649"" supported=""false"" name=""Veeam Backup and Replication 9.0 Beta 1""/>
  <release version=""9.0.0.773""  update=""0"" database=""2750"" supported=""true""  name=""Veeam Backup and Replication 9.0""/>
  <release version=""9.0.0.902""  update=""0"" database=""2754"" supported=""true""  name=""Veeam Backup and Replication 9.0""/>
  <release version=""9.0.0.1483"" update=""1"" database=""2773"" supported=""false"" name=""Veeam Backup and Replication 9.0 Update 1""/>
  <release version=""9.0.0.1491"" update=""1"" database=""2773"" supported=""true""  name=""Veeam Backup and Replication 9.0 Update 1""/>
  <release version=""9.0.0.1715"" update=""2"" database=""2791"" supported=""true""  name=""Veeam Backup and Replication 9.0 Update 2""/>

  <!-- 9.5 -->
  <release version=""9.5.0.221""  update=""0"" database=""3232"" supported=""false"" name=""Veeam Backup and Replication 9.5 Beta 1""/>
  <release version=""9.5.0.348""  update=""0"" database=""3379"" supported=""false"" name=""Veeam Backup and Replication 9.5 Beta 2""/>
  <release version=""9.5.0.580""  update=""0"" database=""3530"" supported=""true""  name=""Veeam Backup and Replication 9.5""/>
  <release version=""9.5.0.711""  update=""0"" database=""3539"" supported=""true""  name=""Veeam Backup and Replication 9.5""/>
  <release version=""9.5.0.802""  update=""1"" database=""3627"" supported=""true""  name=""Veeam Backup and Replication 9.5 Update 1""/>
  <release version=""9.5.0.823""  update=""1"" database=""3628"" supported=""true""  name=""Veeam Backup and Replication 9.5 Update 1""/>
  <release version=""9.5.0.1038"" update=""2"" database=""3701"" supported=""true""  name=""Veeam Backup and Replication 9.5 Update 2""/>
  <release version=""9.5.0.1046"" update=""2"" database=""3701"" supported=""true""  name=""Veeam Backup and Replication 9.5 Update 2""/>
  <release version=""9.5.0.1335"" update=""3"" database=""4033"" supported=""true""  name=""Veeam Backup and Replication 9.5 Update 3""/>
  <release version=""9.5.0.1536"" update=""3"" database=""4051"" supported=""true""  name=""Veeam Backup and Replication 9.5 Update 3""/>
  <release version=""9.5.0.1922"" update=""4"" database=""4087"" supported=""true""  name=""Veeam Backup and Replication 9.5 Update 3a""/>

  <!-- Current -->
  <release version=""9.5.4.2176"" update=""5"" database=""5006"" supported=""false"" name=""Veeam Backup and Replication 9.5 Update 4 Preview 1""/>
  <release version=""9.5.4.2282"" update=""5"" database=""5170"" supported=""false"" name=""Veeam Backup and Replication 9.5 Update 4 Beta 1""/>
  <release version=""9.5.4.2399"" update=""5"" database=""5349"" supported=""true""  name=""Veeam Backup and Replication 9.5 Update 4""/>
  <release version=""0.0.0.0""    update=""5"" database=""0""    supported=""true""  name=""Veeam Backup and Replication 9.5 Update 4""/>
</product>";
  }
}
