using System;

namespace Metropass.Core.PCL.Model
{
    public enum PwCompressionAlgorithm
    {
        /// <summary>
        /// No compression.
        /// </summary>
        None = 0,

        /// <summary>
        /// GZip compression.
        /// </summary>
        GZip = 1,

        /// <summary>
        /// Virtual field: currently known number of algorithms. Should not be used
        /// by plugins or libraries -- it's used internally only.
        /// </summary>
        Count = 2
    }

    /// <summary>
    /// Tree traversal methods.
    /// </summary>
    public enum TraversalMethod
    {
        /// <summary>
        /// Don't traverse the tree.
        /// </summary>
        None = 0,

        /// <summary>
        /// Traverse the tree in pre-order mode, i.e. first visit all items
        /// in the current node, then visit all subnodes.
        /// </summary>
        PreOrder = 1
    }

    /// <summary>
    /// Methods for merging password databases/entries.
    /// </summary>
    public enum PwMergeMethod
    {
        None = 0,
        OverwriteExisting = 1,
        KeepExisting = 2,
        OverwriteIfNewer = 3,
        CreateNewUuids = 4,
        Synchronize = 5
    }

    /// <summary>
    /// Icon identifiers for groups and password entries.
    /// </summary>
    public enum PwIcon
    {
        Key = 0,
        World,
        Warning,
        NetworkServer,
        MarkedDirectory,
        UserCommunication,
        Parts,
        Notepad,
        WorldSocket,
        Identity,
        PaperReady,
        Digicam,
        IRCommunication,
        MultiKeys,
        Energy,
        Scanner,
        WorldStar,
        CDRom,
        Monitor,
        EMail,
        Configuration,
        ClipboardReady,
        PaperNew,
        Screen,
        EnergyCareful,
        EMailBox,
        Disk,
        Drive,
        PaperQ,
        TerminalEncrypted,
        Console,
        Printer,
        ProgramIcons,
        Run,
        Settings,
        WorldComputer,
        Archive,
        Homebanking,
        DriveWindows,
        Clock,
        EMailSearch,
        PaperFlag,
        Memory,
        TrashBin,
        Note,
        Expired,
        Info,
        Package,
        Folder,
        FolderOpen,
        FolderPackage,
        LockOpen,
        PaperLocked,
        Checked,
        Pen,
        Thumbnail,
        Book,
        List,
        UserKey,
        Tool,
        Home,
        Star,
        Tux,
        Feather,
        Apple,
        Wiki,
        Money,
        Certificate,
        BlackBerry,

        /// <summary>
        /// Virtual identifier -- represents the number of icons.
        /// </summary>
        Count
    }

    public enum ProxyServerType
    {
        None = 0,
        System = 1,
        Manual = 2
    }

    /// <summary>
    /// Comparison modes for in-memory protected objects.
    /// </summary>
    public enum MemProtCmpMode
    {
        /// <summary>
        /// Ignore the in-memory protection states.
        /// </summary>
        None = 0,

        /// <summary>
        /// Ignore the in-memory protection states of standard
        /// objects; do compare in-memory protection states of
        /// custom objects.
        /// </summary>
        CustomOnly,

        /// <summary>
        /// Compare in-memory protection states.
        /// </summary>
        Full
    }

    [Flags]
    public enum PwCompareOptions
    {
        None = 0x0,

        /// <summary>
        /// Empty standard string fields are considered to be the
        /// same as non-existing standard string fields.
        /// This doesn't affect custom string comparisons.
        /// </summary>
        NullEmptyEquivStd = 0x1,

        IgnoreParentGroup = 0x2,
        IgnoreLastAccess = 0x4,
        IgnoreLastMod = 0x8,
        IgnoreHistory = 0x10,
        IgnoreLastBackup = 0x20,

        IgnoreTimes = (IgnoreLastAccess | IgnoreLastMod)
    }
}
