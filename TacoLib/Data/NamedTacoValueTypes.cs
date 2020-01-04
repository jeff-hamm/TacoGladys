using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TacoLib.Data
{
    public enum TacoValueId
    {
             [Display(Name="CYLINDER ID")]
        CYLID,
     [Display(Name="RPM PERIOD")]
        RPMPERIOD,
     [Display(Name="TARGET IDLE SPEED")]
        IDLESPD,
     [Display(Name="SCALED RPM")]
        RPM,
     [Display(Name="THROTTLE POSITION")]
        TPS,
     [Display(Name="COOLANT TEMPERATURE")]
        COOLTMP,
     [Display(Name="BLM CELL NUMBER")]
        CELL,
     [Display(Name="LEFT BLM")]
        LBLM,
     [Display(Name="RIGHT BLM")]
        RBLM,
     [Display(Name="MANIFOLD PRESSURE")]
        MAP,
     [Display(Name="LEFT O2 SENSOR VOLTAGE")]
        LO2,
     [Display(Name="RIGHT O2 SENSOR VOLTAGE")]
        RO2,
     [Display(Name="INTAKE AIR TEMPERATURE")]
        IAT,
     [Display(Name="SPARK OUTPUT")]
        ADV,
     [Display(Name="VEHICLE SPEED")]
        SPEED,
     [Display(Name="KNOCK RETARD")]
        KR,
     [Display(Name="KNOCK SENSOR COUNTS")]
        KNOCK,
     [Display(Name="IAC MOTOR POSITION")]
        IAC,
     [Display(Name="ATMOSPHERIC PRESSURE")]
        BARO,
     [Display(Name="FAN 1 RELAY")]
        FAN1,
     [Display(Name="FAN 2 RELAY")]
        FAN2,
     [Display(Name="PCM VOLTAGE")]
        ADIGNVLT,
     [Display(Name="LEFT INTEGRATOR")]
        LINT,
     [Display(Name="RIGHT INTEGRATOR")]
        RINT,
     [Display(Name="MAF AIRFLOW")]
        MAF,
     [Display(Name="LEFT BASE INJETOR PULSE WIDTH")]
        LBPW,
     [Display(Name="RIGHT BASE INJETOR PULSE WIDTH")]
        RBPW,
     [Display(Name="CLOSED LOOP ENABLE")]
        CL,
     [Display(Name="BLOCK LEARN ENABLED")]
        BLM,
     [Display(Name="POWER ENRICH MODE ENABLED")]
        PE,
     [Display(Name="FUEL SYSTEM LEAN")]
        CODE_55,
     [Display(Name="SKIP SHIFT CKT (ODM)")]
     CODE_84,
     [Display(Name="REVERSE INHIBIT CKT (ODM)")]
     CODE_83,
     [Display(Name="EGR ELECTRICAL FAULT (ODM)")]
     CODE_27,
     [Display(Name="CCP ELECTRICAL FAULT (ODM)")]
     CODE_26,
     [Display(Name="MAT SENSOR HIGH")]
     CODE_25,
     [Display(Name="MAT SENSOR LOW")]
     CODE_23,
     [Display(Name="LO-RES FAILURE")]
     CODE_16,
     [Display(Name="LEFT O2 SENSOR FAILED")]
     CODE_13,
     [Display(Name="NO REFERENCE PULSES")]
     CODE_12,
     [Display(Name="MALF INDICATOR LAMP (MIL) FAULT")]
     CODE_11,
     [Display(Name="ESC FAILURE")]
     CODE_43,
     [Display(Name="EST GROUNDED")]
     CODE_42,
     [Display(Name="EST OPEN")]
     CODE_41,
     [Display(Name="HI-RES FAILURE")]
     CODE_36,
     [Display(Name="MASS AIR FLOW SYSTEM FAILURE")]
     CODE_48,
     [Display(Name="EGR SYSTEM FAULT")]
     CODE_32,
     [Display(Name="EAS ELECTRICAL FAULT (ODM)")]
     CODE_29,
     [Display(Name="SYSTEM VOLTAGE LOW")]
     CODE_50,
     [Display(Name="FUEL ENABLE FAILURE")]
     CODE_46,
     [Display(Name="LEFT O2 SENSOR RICH")]
     CODE_45,
     [Display(Name="LEFT O2 SENSOR LEAN")]
     CODE_44,
     [Display(Name="RIGHT O2 SENSOR RICH")]
     CODE_65,
     [Display(Name="RIGHT O2 SENSOR LEAN")]
     CODE_64,
     [Display(Name="RIGHT O2 SENSOR FAILED")]
     CODE_63,
     [Display(Name="FAN 2 FAULT (ODM)")]
     CODE_78,
     [Display(Name="FAN 1 FAULT (ODM)")]
     CODE_77,
     [Display(Name="MAP SENSOR LOW")]
     CODE_34,
     [Display(Name="MAP SENSOR HIGH")]
     CODE_33,
     [Display(Name="OUTPUT SPEED LOW")]
     CODE_24,
     [Display(Name="SYSTEM VOLTAGE HIGH")]
     CODE_53,
     [Display(Name="PROM/FLASH ERROR")]
     CODE_51,
     [Display(Name="THROTTLE POSITION LOW")]
     CODE_22,
     [Display(Name="THROTTLE POSITION HIGH")]
     CODE_21,
     [Display(Name="COOLANT TEMPERATURE LOW")]
     CODE_15,
     [Display(Name="COOLANT TEMPERATURE HIGH")]
     CODE_14,
     [Display(Name="TACH OUTPUT CKT FAULT")]
     CODE_99,
     [Display(Name="4K PULSES CKT FAULT (VSS)")]
     CODE_97,
     [Display(Name="WB AFR")]
        AFR,

    }
}
