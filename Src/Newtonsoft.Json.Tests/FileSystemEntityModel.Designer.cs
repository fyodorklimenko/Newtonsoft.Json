﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

[assembly: EdmSchemaAttribute()]
#region EDM Relationship Metadata

[assembly: EdmRelationshipAttribute("DataServicesTestDatabaseModel", "FK_File_Folder", "Folder", System.Data.Metadata.Edm.RelationshipMultiplicity.One, typeof(Newtonsoft.Json.Tests.Folder), "File", System.Data.Metadata.Edm.RelationshipMultiplicity.Many, typeof(Newtonsoft.Json.Tests.File))]
[assembly: EdmRelationshipAttribute("DataServicesTestDatabaseModel", "FK_Folder_Folder", "Folder", System.Data.Metadata.Edm.RelationshipMultiplicity.ZeroOrOne, typeof(Newtonsoft.Json.Tests.Folder), "Folder1", System.Data.Metadata.Edm.RelationshipMultiplicity.Many, typeof(Newtonsoft.Json.Tests.Folder))]

#endregion

namespace Newtonsoft.Json.Tests
{
    #region Contexts
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    public partial class DataServicesTestDatabaseEntities : ObjectContext
    {
        #region Constructors
    
        /// <summary>
        /// Initializes a new DataServicesTestDatabaseEntities object using the connection string found in the 'DataServicesTestDatabaseEntities' section of the application configuration file.
        /// </summary>
        public DataServicesTestDatabaseEntities() : base("name=DataServicesTestDatabaseEntities", "DataServicesTestDatabaseEntities")
        {
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new DataServicesTestDatabaseEntities object.
        /// </summary>
        public DataServicesTestDatabaseEntities(string connectionString) : base(connectionString, "DataServicesTestDatabaseEntities")
        {
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new DataServicesTestDatabaseEntities object.
        /// </summary>
        public DataServicesTestDatabaseEntities(EntityConnection connection) : base(connection, "DataServicesTestDatabaseEntities")
        {
            OnContextCreated();
        }
    
        #endregion
    
        #region Partial Methods
    
        partial void OnContextCreated();
    
        #endregion
    
        #region ObjectSet Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<File> File
        {
            get
            {
                if ((_File == null))
                {
                    _File = base.CreateObjectSet<File>("File");
                }
                return _File;
            }
        }
        private ObjectSet<File> _File;
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<Folder> Folder
        {
            get
            {
                if ((_Folder == null))
                {
                    _Folder = base.CreateObjectSet<Folder>("Folder");
                }
                return _Folder;
            }
        }
        private ObjectSet<Folder> _Folder;

        #endregion

        #region AddTo Methods
    
        /// <summary>
        /// Deprecated Method for adding a new object to the File EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToFile(File file)
        {
            base.AddObject("File", file);
        }
    
        /// <summary>
        /// Deprecated Method for adding a new object to the Folder EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToFolder(Folder folder)
        {
            base.AddObject("Folder", folder);
        }

        #endregion

    }

    #endregion

    #region Entities
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="DataServicesTestDatabaseModel", Name="File")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class File : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new File object.
        /// </summary>
        /// <param name="fileId">Initial value of the FileId property.</param>
        /// <param name="name">Initial value of the Name property.</param>
        /// <param name="description">Initial value of the Description property.</param>
        /// <param name="createdDate">Initial value of the CreatedDate property.</param>
        public static File CreateFile(global::System.Guid fileId, global::System.String name, global::System.String description, global::System.DateTime createdDate)
        {
            File file = new File();
            file.FileId = fileId;
            file.Name = name;
            file.Description = description;
            file.CreatedDate = createdDate;
            return file;
        }

        #endregion

        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Guid FileId
        {
            get
            {
                return _FileId;
            }
            set
            {
                if (_FileId != value)
                {
                    OnFileIdChanging(value);
                    ReportPropertyChanging("FileId");
                    _FileId = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("FileId");
                    OnFileIdChanged();
                }
            }
        }
        private global::System.Guid _FileId;
        partial void OnFileIdChanging(global::System.Guid value);
        partial void OnFileIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Name
        {
            get
            {
                return _Name;
            }
            set
            {
                OnNameChanging(value);
                ReportPropertyChanging("Name");
                _Name = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("Name");
                OnNameChanged();
            }
        }
        private global::System.String _Name;
        partial void OnNameChanging(global::System.String value);
        partial void OnNameChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Description
        {
            get
            {
                return _Description;
            }
            set
            {
                OnDescriptionChanging(value);
                ReportPropertyChanging("Description");
                _Description = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("Description");
                OnDescriptionChanged();
            }
        }
        private global::System.String _Description;
        partial void OnDescriptionChanging(global::System.String value);
        partial void OnDescriptionChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.DateTime CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                OnCreatedDateChanging(value);
                ReportPropertyChanging("CreatedDate");
                _CreatedDate = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("CreatedDate");
                OnCreatedDateChanged();
            }
        }
        private global::System.DateTime _CreatedDate;
        partial void OnCreatedDateChanging(global::System.DateTime value);
        partial void OnCreatedDateChanged();

        #endregion

    
        #region Navigation Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [XmlIgnoreAttribute()]
        [SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        [EdmRelationshipNavigationPropertyAttribute("DataServicesTestDatabaseModel", "FK_File_Folder", "Folder")]
        public Folder Folder
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<Folder>("DataServicesTestDatabaseModel.FK_File_Folder", "Folder").Value;
            }
            set
            {
                ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<Folder>("DataServicesTestDatabaseModel.FK_File_Folder", "Folder").Value = value;
            }
        }
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [BrowsableAttribute(false)]
        [DataMemberAttribute()]
        public EntityReference<Folder> FolderReference
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<Folder>("DataServicesTestDatabaseModel.FK_File_Folder", "Folder");
            }
            set
            {
                if ((value != null))
                {
                    ((IEntityWithRelationships)this).RelationshipManager.InitializeRelatedReference<Folder>("DataServicesTestDatabaseModel.FK_File_Folder", "Folder", value);
                }
            }
        }

        #endregion

    }
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="DataServicesTestDatabaseModel", Name="Folder")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class Folder : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new Folder object.
        /// </summary>
        /// <param name="folderId">Initial value of the FolderId property.</param>
        /// <param name="name">Initial value of the Name property.</param>
        /// <param name="description">Initial value of the Description property.</param>
        /// <param name="createdDate">Initial value of the CreatedDate property.</param>
        public static Folder CreateFolder(global::System.Guid folderId, global::System.String name, global::System.String description, global::System.DateTime createdDate)
        {
            Folder folder = new Folder();
            folder.FolderId = folderId;
            folder.Name = name;
            folder.Description = description;
            folder.CreatedDate = createdDate;
            return folder;
        }

        #endregion

        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Guid FolderId
        {
            get
            {
                return _FolderId;
            }
            set
            {
                if (_FolderId != value)
                {
                    OnFolderIdChanging(value);
                    ReportPropertyChanging("FolderId");
                    _FolderId = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("FolderId");
                    OnFolderIdChanged();
                }
            }
        }
        private global::System.Guid _FolderId;
        partial void OnFolderIdChanging(global::System.Guid value);
        partial void OnFolderIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Name
        {
            get
            {
                return _Name;
            }
            set
            {
                OnNameChanging(value);
                ReportPropertyChanging("Name");
                _Name = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("Name");
                OnNameChanged();
            }
        }
        private global::System.String _Name;
        partial void OnNameChanging(global::System.String value);
        partial void OnNameChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Description
        {
            get
            {
                return _Description;
            }
            set
            {
                OnDescriptionChanging(value);
                ReportPropertyChanging("Description");
                _Description = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("Description");
                OnDescriptionChanged();
            }
        }
        private global::System.String _Description;
        partial void OnDescriptionChanging(global::System.String value);
        partial void OnDescriptionChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.DateTime CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                OnCreatedDateChanging(value);
                ReportPropertyChanging("CreatedDate");
                _CreatedDate = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("CreatedDate");
                OnCreatedDateChanged();
            }
        }
        private global::System.DateTime _CreatedDate;
        partial void OnCreatedDateChanging(global::System.DateTime value);
        partial void OnCreatedDateChanged();

        #endregion

    
        #region Navigation Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [XmlIgnoreAttribute()]
        [SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        [EdmRelationshipNavigationPropertyAttribute("DataServicesTestDatabaseModel", "FK_File_Folder", "File")]
        public EntityCollection<File> Files
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedCollection<File>("DataServicesTestDatabaseModel.FK_File_Folder", "File");
            }
            set
            {
                if ((value != null))
                {
                    ((IEntityWithRelationships)this).RelationshipManager.InitializeRelatedCollection<File>("DataServicesTestDatabaseModel.FK_File_Folder", "File", value);
                }
            }
        }
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [XmlIgnoreAttribute()]
        [SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        [EdmRelationshipNavigationPropertyAttribute("DataServicesTestDatabaseModel", "FK_Folder_Folder", "Folder1")]
        public EntityCollection<Folder> ChildFolders
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedCollection<Folder>("DataServicesTestDatabaseModel.FK_Folder_Folder", "Folder1");
            }
            set
            {
                if ((value != null))
                {
                    ((IEntityWithRelationships)this).RelationshipManager.InitializeRelatedCollection<Folder>("DataServicesTestDatabaseModel.FK_Folder_Folder", "Folder1", value);
                }
            }
        }
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [XmlIgnoreAttribute()]
        [SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        [EdmRelationshipNavigationPropertyAttribute("DataServicesTestDatabaseModel", "FK_Folder_Folder", "Folder")]
        public Folder ParentFolder
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<Folder>("DataServicesTestDatabaseModel.FK_Folder_Folder", "Folder").Value;
            }
            set
            {
                ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<Folder>("DataServicesTestDatabaseModel.FK_Folder_Folder", "Folder").Value = value;
            }
        }
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [BrowsableAttribute(false)]
        [DataMemberAttribute()]
        public EntityReference<Folder> ParentFolderReference
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<Folder>("DataServicesTestDatabaseModel.FK_Folder_Folder", "Folder");
            }
            set
            {
                if ((value != null))
                {
                    ((IEntityWithRelationships)this).RelationshipManager.InitializeRelatedReference<Folder>("DataServicesTestDatabaseModel.FK_Folder_Folder", "Folder", value);
                }
            }
        }

        #endregion

    }

    #endregion

    
}
