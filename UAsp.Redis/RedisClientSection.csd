<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="7801ffa7-d7cb-4b8e-99de-a0d8298f4bb1" namespace="UAsp.Redis" xmlSchemaNamespace="urn:UAsp.Redis" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
  <typeDefinitions>
    <externalType name="String" namespace="System" />
    <externalType name="Boolean" namespace="System" />
    <externalType name="Int32" namespace="System" />
    <externalType name="Int64" namespace="System" />
    <externalType name="Single" namespace="System" />
    <externalType name="Double" namespace="System" />
    <externalType name="DateTime" namespace="System" />
    <externalType name="TimeSpan" namespace="System" />
  </typeDefinitions>
  <configurationElements>
    <configurationSection name="RedisSection" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="redisSection">
      <attributeProperties>
        <attributeProperty name="Db" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="db" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/7801ffa7-d7cb-4b8e-99de-a0d8298f4bb1/Int32" />
          </type>
        </attributeProperty>
        <attributeProperty name="Password" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="password" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/7801ffa7-d7cb-4b8e-99de-a0d8298f4bb1/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Cluster" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="cluster" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/7801ffa7-d7cb-4b8e-99de-a0d8298f4bb1/Boolean" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <elementProperties>
        <elementProperty name="Write" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="write" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/7801ffa7-d7cb-4b8e-99de-a0d8298f4bb1/ItemCollection" />
          </type>
        </elementProperty>
        <elementProperty name="Read" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="read" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/7801ffa7-d7cb-4b8e-99de-a0d8298f4bb1/ItemCollection" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElementCollection name="ItemCollection" collectionType="AddRemoveClearMapAlternate" xmlItemName="hostItem" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/7801ffa7-d7cb-4b8e-99de-a0d8298f4bb1/Host" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="Host">
      <attributeProperties>
        <attributeProperty name="Ip" isRequired="false" isKey="false" isDefaultCollection="true" xmlName="ip" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/7801ffa7-d7cb-4b8e-99de-a0d8298f4bb1/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Port" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="port" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/7801ffa7-d7cb-4b8e-99de-a0d8298f4bb1/Int32" />
          </type>
        </attributeProperty>
        <attributeProperty name="Pool" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="pool" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/7801ffa7-d7cb-4b8e-99de-a0d8298f4bb1/Int32" />
          </type>
        </attributeProperty>
        <attributeProperty name="Timeout" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="timeout" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/7801ffa7-d7cb-4b8e-99de-a0d8298f4bb1/Int32" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>