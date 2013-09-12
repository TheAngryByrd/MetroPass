using System.Xml.Linq;

namespace Metropass.Core.PCL
{
    public class GenerateDatabase
    {
        public static XDocument NewDatabase()
        {
            var databaseXML = @"<KeePassFile>
  <Meta>
    <Generator>KeePass</Generator>
    <HeaderHash>RvugqE4SC4JyWRu8xly+aZ26xRH8U6lFYMoPqJEAC9A=</HeaderHash>
    <DatabaseName />
    <DatabaseNameChanged>2012-10-26T01:36:46Z</DatabaseNameChanged>
    <DatabaseDescription />
    <DatabaseDescriptionChanged>2012-10-26T01:36:46Z</DatabaseDescriptionChanged>
    <DefaultUserName />
    <DefaultUserNameChanged>2012-10-26T01:36:46Z</DefaultUserNameChanged>
    <MaintenanceHistoryDays>365</MaintenanceHistoryDays>
    <Color />
    <MasterKeyChanged>2012-10-26T01:36:46Z</MasterKeyChanged>
    <MasterKeyChangeRec>-1</MasterKeyChangeRec>
    <MasterKeyChangeForce>-1</MasterKeyChangeForce>
    <MemoryProtection>
      <ProtectTitle>False</ProtectTitle>
      <ProtectUserName>False</ProtectUserName>
      <ProtectPassword>True</ProtectPassword>
      <ProtectURL>False</ProtectURL>
      <ProtectNotes>False</ProtectNotes>
    </MemoryProtection>
    <RecycleBinEnabled>True</RecycleBinEnabled>
    <RecycleBinUUID>AAAAAAAAAAAAAAAAAAAAAA==</RecycleBinUUID>
    <RecycleBinChanged>2012-10-26T01:36:46Z</RecycleBinChanged>
    <EntryTemplatesGroup>AAAAAAAAAAAAAAAAAAAAAA==</EntryTemplatesGroup>
    <EntryTemplatesGroupChanged>2012-10-26T01:36:46Z</EntryTemplatesGroupChanged>
    <HistoryMaxItems>10</HistoryMaxItems>
    <HistoryMaxSize>6291456</HistoryMaxSize>
    <LastSelectedGroup>LSxS3qVbmEuUlTtyp16Z2g==</LastSelectedGroup>
    <LastTopVisibleGroup>LSxS3qVbmEuUlTtyp16Z2g==</LastTopVisibleGroup>
    <Binaries />
    <CustomData />
  </Meta>
  <Root>
    <Group>
      <UUID>LSxS3qVbmEuUlTtyp16Z2g==</UUID>
      <Name>NewDatabase</Name>
      <Notes />
      <IconID>49</IconID>
      <Times>
        <LastModificationTime>2012-10-26T01:36:46Z</LastModificationTime>
        <CreationTime>2012-10-26T01:36:46Z</CreationTime>
        <LastAccessTime>2012-10-26T01:36:46Z</LastAccessTime>
        <ExpiryTime>2012-10-26T01:36:09Z</ExpiryTime>
        <Expires>False</Expires>
        <UsageCount>0</UsageCount>
        <LocationChanged>2012-10-26T01:36:46Z</LocationChanged>
      </Times>
      <IsExpanded>True</IsExpanded>
      <DefaultAutoTypeSequence />
      <EnableAutoType>null</EnableAutoType>
      <EnableSearching>null</EnableSearching>
      <LastTopVisibleEntry>4uGC2PWNBES1kVlz0KVLcw==</LastTopVisibleEntry>
      <Entry>
        <UUID>4uGC2PWNBES1kVlz0KVLcw==</UUID>
        <IconID>0</IconID>
        <ForegroundColor />
        <BackgroundColor />
        <OverrideURL />
        <Tags />
        <Times>
          <LastModificationTime>2012-10-26T01:36:48Z</LastModificationTime>
          <CreationTime>2012-10-26T01:36:48Z</CreationTime>
          <LastAccessTime>2012-10-26T01:36:48Z</LastAccessTime>
          <ExpiryTime>2012-10-26T01:36:09Z</ExpiryTime>
          <Expires>False</Expires>
          <UsageCount>0</UsageCount>
          <LocationChanged>2012-10-26T01:36:48Z</LocationChanged>
        </Times>
        <String>
          <Key>Notes</Key>
          <Value>Notes</Value>
        </String>
        <String>
          <Key>Password</Key>
          <Value Protected=""True"">Password</Value>
        </String>
        <String>
          <Key>Title</Key>
          <Value>Sample Entry</Value>
        </String>
        <String>
          <Key>URL</Key>
          <Value>http://keepass.info/</Value>
        </String>
        <String>
          <Key>UserName</Key>
          <Value>User Name</Value>
        </String>
        <AutoType>
          <Enabled>True</Enabled>
          <DataTransferObfuscation>0</DataTransferObfuscation>
          <Association>
            <Window>Target Window</Window>
            <KeystrokeSequence>{USERNAME}{TAB}{PASSWORD}{TAB}{ENTER}</KeystrokeSequence>
          </Association>
        </AutoType>
        <History />
      </Entry>
      <Entry>
        <UUID>gz7HMdAMEkanjNxZVqERqQ==</UUID>
        <IconID>0</IconID>
        <ForegroundColor />
        <BackgroundColor />
        <OverrideURL />
        <Tags />
        <Times>
          <LastModificationTime>2012-10-26T01:36:48Z</LastModificationTime>
          <CreationTime>2012-10-26T01:36:48Z</CreationTime>
          <LastAccessTime>2012-10-26T01:36:48Z</LastAccessTime>
          <ExpiryTime>2012-10-26T01:36:09Z</ExpiryTime>
          <Expires>False</Expires>
          <UsageCount>0</UsageCount>
          <LocationChanged>2012-10-26T01:36:48Z</LocationChanged>
        </Times>
        <String>
          <Key>Password</Key>
          <Value Protected=""True"">12345</Value>
        </String>
        <String>
          <Key>Title</Key>
          <Value>Sample Entry #2</Value>
        </String>
        <String>
          <Key>URL</Key>
          <Value>http://keepass.info/help/kb/kb090406_testform.html</Value>
        </String>
        <String>
          <Key>UserName</Key>
          <Value>Michael321</Value>
        </String>
        <AutoType>
          <Enabled>True</Enabled>
          <DataTransferObfuscation>0</DataTransferObfuscation>
          <Association>
            <Window>Test Form - KeePass*</Window>
            <KeystrokeSequence />
          </Association>
        </AutoType>
        <History />
      </Entry>
      <Group>
        <UUID>0hRUbkcZ7UqSEZVN5Vqi+w==</UUID>
        <Name>General</Name>
        <Notes />
        <IconID>48</IconID>
        <Times>
          <LastModificationTime>2012-10-26T01:36:48Z</LastModificationTime>
          <CreationTime>2012-10-26T01:36:48Z</CreationTime>
          <LastAccessTime>2012-10-26T01:36:48Z</LastAccessTime>
          <ExpiryTime>2012-10-26T01:36:09Z</ExpiryTime>
          <Expires>False</Expires>
          <UsageCount>0</UsageCount>
          <LocationChanged>2012-10-26T01:36:48Z</LocationChanged>
        </Times>
        <IsExpanded>True</IsExpanded>
        <DefaultAutoTypeSequence />
        <EnableAutoType>null</EnableAutoType>
        <EnableSearching>null</EnableSearching>
        <LastTopVisibleEntry>AAAAAAAAAAAAAAAAAAAAAA==</LastTopVisibleEntry>
      </Group>
      <Group>
        <UUID>A2z/ePfvjUGUQ3NqQ6gmCQ==</UUID>
        <Name>Windows</Name>
        <Notes />
        <IconID>38</IconID>
        <Times>
          <LastModificationTime>2012-10-26T01:36:48Z</LastModificationTime>
          <CreationTime>2012-10-26T01:36:48Z</CreationTime>
          <LastAccessTime>2012-10-26T01:36:48Z</LastAccessTime>
          <ExpiryTime>2012-10-26T01:36:09Z</ExpiryTime>
          <Expires>False</Expires>
          <UsageCount>0</UsageCount>
          <LocationChanged>2012-10-26T01:36:48Z</LocationChanged>
        </Times>
        <IsExpanded>True</IsExpanded>
        <DefaultAutoTypeSequence />
        <EnableAutoType>null</EnableAutoType>
        <EnableSearching>null</EnableSearching>
        <LastTopVisibleEntry>AAAAAAAAAAAAAAAAAAAAAA==</LastTopVisibleEntry>
      </Group>
      <Group>
        <UUID>1SNhZcX8Bk+R5P3jlNbXFw==</UUID>
        <Name>Network</Name>
        <Notes />
        <IconID>3</IconID>
        <Times>
          <LastModificationTime>2012-10-26T01:36:48Z</LastModificationTime>
          <CreationTime>2012-10-26T01:36:48Z</CreationTime>
          <LastAccessTime>2012-10-26T01:36:48Z</LastAccessTime>
          <ExpiryTime>2012-10-26T01:36:09Z</ExpiryTime>
          <Expires>False</Expires>
          <UsageCount>0</UsageCount>
          <LocationChanged>2012-10-26T01:36:48Z</LocationChanged>
        </Times>
        <IsExpanded>True</IsExpanded>
        <DefaultAutoTypeSequence />
        <EnableAutoType>null</EnableAutoType>
        <EnableSearching>null</EnableSearching>
        <LastTopVisibleEntry>AAAAAAAAAAAAAAAAAAAAAA==</LastTopVisibleEntry>
      </Group>
      <Group>
        <UUID>fAf0YdGyQUS1bjE4ve2u2A==</UUID>
        <Name>Internet</Name>
        <Notes />
        <IconID>1</IconID>
        <Times>
          <LastModificationTime>2012-10-26T01:36:48Z</LastModificationTime>
          <CreationTime>2012-10-26T01:36:48Z</CreationTime>
          <LastAccessTime>2012-10-26T01:36:48Z</LastAccessTime>
          <ExpiryTime>2012-10-26T01:36:09Z</ExpiryTime>
          <Expires>False</Expires>
          <UsageCount>0</UsageCount>
          <LocationChanged>2012-10-26T01:36:48Z</LocationChanged>
        </Times>
        <IsExpanded>True</IsExpanded>
        <DefaultAutoTypeSequence />
        <EnableAutoType>null</EnableAutoType>
        <EnableSearching>null</EnableSearching>
        <LastTopVisibleEntry>AAAAAAAAAAAAAAAAAAAAAA==</LastTopVisibleEntry>
      </Group>
      <Group>
        <UUID>v0sgmDTy+kyZzFCWMkA+GA==</UUID>
        <Name>eMail</Name>
        <Notes />
        <IconID>19</IconID>
        <Times>
          <LastModificationTime>2012-10-26T01:36:48Z</LastModificationTime>
          <CreationTime>2012-10-26T01:36:48Z</CreationTime>
          <LastAccessTime>2012-10-26T01:36:48Z</LastAccessTime>
          <ExpiryTime>2012-10-26T01:36:09Z</ExpiryTime>
          <Expires>False</Expires>
          <UsageCount>0</UsageCount>
          <LocationChanged>2012-10-26T01:36:48Z</LocationChanged>
        </Times>
        <IsExpanded>True</IsExpanded>
        <DefaultAutoTypeSequence />
        <EnableAutoType>null</EnableAutoType>
        <EnableSearching>null</EnableSearching>
        <LastTopVisibleEntry>AAAAAAAAAAAAAAAAAAAAAA==</LastTopVisibleEntry>
      </Group>
      <Group>
        <UUID>NFAifcpsj0CdjPhL8yxZGg==</UUID>
        <Name>Homebanking</Name>
        <Notes />
        <IconID>37</IconID>
        <Times>
          <LastModificationTime>2012-10-26T01:36:48Z</LastModificationTime>
          <CreationTime>2012-10-26T01:36:48Z</CreationTime>
          <LastAccessTime>2012-10-26T01:36:48Z</LastAccessTime>
          <ExpiryTime>2012-10-26T01:36:09Z</ExpiryTime>
          <Expires>False</Expires>
          <UsageCount>0</UsageCount>
          <LocationChanged>2012-10-26T01:36:48Z</LocationChanged>
        </Times>
        <IsExpanded>True</IsExpanded>
        <DefaultAutoTypeSequence />
        <EnableAutoType>null</EnableAutoType>
        <EnableSearching>null</EnableSearching>
        <LastTopVisibleEntry>AAAAAAAAAAAAAAAAAAAAAA==</LastTopVisibleEntry>
      </Group>
    </Group>
    <DeletedObjects />
  </Root>
</KeePassFile>";
            

            var formatedXml = databaseXML;
            return XDocument.Parse(formatedXml);
        }
    }
}
