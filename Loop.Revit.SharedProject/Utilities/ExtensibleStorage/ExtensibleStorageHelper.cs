﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace Loop.Revit.Utilities.ExtensibleStorage
{
    public static class ExtensibleStorageHelper
    {

        public static Schema CreateSimpleSchema(Guid guid, string schemaName, string schemaDescription, List<(string fieldName, Type fieldType)> fieldInfo, ForgeTypeId specTypeId = null)
        {
            //TODO: split the lookup into a different method.
            var schema = Schema.Lookup(guid);

            if (schema != null)
            {
                return schema;
            }

            var schemaBuilder = new SchemaBuilder(guid);
            schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
            schemaBuilder.SetWriteAccessLevel(AccessLevel.Public);
            schemaBuilder.SetSchemaName(schemaName);
            schemaBuilder.SetDocumentation(schemaDescription);

            foreach (var field in fieldInfo)
            {
                var fieldBuilder = schemaBuilder.AddSimpleField(field.fieldName, field.fieldType);
                if (specTypeId != null) fieldBuilder.SetSpec(specTypeId);
            }

            var newSchema = schemaBuilder.Finish();

            return newSchema;
        }

        public static DataStorage CreateDataStorage(Document Doc, Schema schema, Guid guid, string storageItemName, List<(string parameterName, dynamic parameterData)> parameters, ForgeTypeId unitTypeId =null)
        {
            var existingDataStorage = new FilteredElementCollector(Doc).OfClass(typeof(DataStorage)).Cast<DataStorage>();
            DataStorage storageItem = null;
            foreach (var element in existingDataStorage)
            {
                if (element.GetEntitySchemaGuids()[0] != guid) continue;
                element.DeleteEntity(schema);
                storageItem = element;
                break;
            }

            if (storageItem == null) storageItem = DataStorage.Create(Doc);

            storageItem.Name = storageItemName;
            var entity = new Entity(schema);

            foreach (var param in parameters)
            {
                if (unitTypeId == null) entity.Set(param.parameterName, param.parameterData);
                else entity.Set(param.parameterName, param.parameterData, unitTypeId);
            }
            storageItem.SetEntity(entity);

            return storageItem;
        }

        public static DataStorage LoadDataStorage(Document Doc, Guid guid)
        {
            var existingDataStorage = new FilteredElementCollector(Doc).OfClass(typeof(DataStorage)).Cast<DataStorage>();
            DataStorage storageItem = null;
            foreach (var element in existingDataStorage)
            {
                if (element.GetEntitySchemaGuids()[0] != guid) continue;
                storageItem = element;
                break;
            }
            return storageItem;
        }


    }
}
