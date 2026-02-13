using System;
using System.Collections.Generic;
using System.Text;

namespace FTN.Common
{
	
	public enum DMSType : short
	{		
		MASK_TYPE							= unchecked((short)0xFFFF),

        // KONKRETNE KLASE
        BASEVOLTAGE                         = 0x0001,
        SWITCH								= 0x0002,
        TERMINAL							= 0x0003,
        CONNECTIVITYNODE					= 0x0004,
        CONNECTIVITYNODECONTAINER			= 0x0005,
        TOPOLOGICALNODE						= 0x0006,

        // APSTRAKTNE KLASE 

        IDJOBJ                              = 0x0007,
        PSR                                 = 0x0008,
        EQUIPMENT                           = 0x0009,
        CONDEQ                              = 0x000A,
    }

    [Flags]
	public enum ModelCode : long
	{
		// APSTRAKTNE KLASE

		IDOBJ								= 0x1000000000000000,
        IDOBJ_GID                           = 0x1000000000000104, //mora
        IDOBJ_ALIASNAME 				    = 0x1000000000000207, // string (7)
		IDOBJ_MRID							= 0x1000000000000307,
		IDOBJ_NAME							= 0x1000000000000407,

        PSR									= 0x1100000000000000,

        EQUIPMENT							= 0x1110000000000000,
        EQUIPMENT_AGGREGATE					= 0x1110000000000101, // bool (1)
        EQUIPMENT_NORMALLYINSERVICE			= 0x1110000000000201, // bool

        CONDEQ								= 0x1111000000000000,
		CONDEQ_BASVOLTAGE					= 0x1111000000000109,
        CONDEQ_TERMINALS					= 0x1111000000000219, // lista Terminala (19)

        // KONKRETNE KLASE

        // BASEVOLTAGE (DMS 0x0001, Nasleđuje IdObj - 1)
        BASEVOLTAGE                         = 0x1200000000010000,
        BASEVOLTAGE_CONDEQ                  = 0x1200000000010119, // lista CondEq (19)

        // SWITCH (DMS 0x0002, Nasleđuje CondEq - 1111)
        SWITCH								= 0x1111100000020000,
        SWITCH_NORMALOPEN					= 0x1111100000020101, // bool (1)
        SWITCH_RATEDCURRENT					= 0x1111100000020205, // float (5)
        SWITCH_RETAINED						= 0x1111100000020301, // bool (1)
        SWITCH_SWITCHONCOUNT				= 0x1111100000020403, // int32 (3)
        SWITCH_SWITCHONDATE					= 0x1111100000020508, // datetime (8)

        // TERMINAL (DMS 0x0003, Nasleđuje IdObj - 1)
        TERMINAL							= 0x1300000000030000,
        TERMINAL_CONDEQ						= 0x1300000000030109, // ref ka CondEq (9)
        TERMINAL_CONNNODE					= 0x1300000000030209, // ref ka ConnNode (9)

        // CONNECTIVITYNODE (DMS 0x0004, Nasleđuje IdObj - 1)
        CONNECTIVITYNODE					= 0x1400000000040000,
        CONNECTIVITYNODE_CONTAINER			= 0x1400000000040109, // ref (9)
        CONNECTIVITYNODE_TERMINALS			= 0x1400000000040219, // lista (19)
        CONNECTIVITYNODE_TOPONODE			= 0x1400000000040309, // ref (9)

        // CONNECTIVITYNODECONTAINER (DMS 0x0005, Nasleđuje PSR - 11)
        CONNECTIVITYNODECONTAINER			= 0x1120000000050000,
        CONNNODECONTAINER_NODES				= 0x1120000000050119, // lista (19)

        // TOPOLOGICALNODE (DMS 0x0006, Nasleđuje IdObj - 1)
        TOPOLOGICALNODE						= 0x1500000000060000,
        TOPOLOGICALNODE_CONNNODES			= 0x1500000000060119, // lista (19)

	}

    [Flags]
	public enum ModelCodeMask : long
	{
		MASK_TYPE			 = 0x00000000ffff0000,
		MASK_ATTRIBUTE_INDEX = 0x000000000000ff00,
		MASK_ATTRIBUTE_TYPE	 = 0x00000000000000ff,

		MASK_INHERITANCE_ONLY = unchecked((long)0xffffffff00000000),
		MASK_FIRSTNBL		  = unchecked((long)0xf000000000000000),
		MASK_DELFROMNBL8	  = unchecked((long)0xfffffff000000000),		
	}																		
}


