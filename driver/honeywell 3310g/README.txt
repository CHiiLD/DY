Honeywell USB Serial Driver Installation


Uncompress the driver install media and save to TARGET COMPUTER as <driver install directory>.
The <driver install directory> is the directory folder where all the files in the driver install media get uncompressed and copied to.
The name of the <driver install directory> can be any valid string name of the user choice.

BEFORE PROCEED, PLEASE CONSULT THE "NOTES" SECTION AT THE BOTTOM OF THIS DOCUMENT FOR SPECIAL USE CASES.

NORMAL(INTERACTIVE) INSTALL/UNINSTALL (for normal user)
=======================================================

1) INSTALLATION: The driver can be installed to target computer by following one of the below methods:

a/ Method 1 -- Install from a command prompt

   i/ Open a Command Prompt with "elevated priviledge". Right click "Start->All Programs->Accessories->Command Prompt". 
      Select "Run as administrator".
  ii/ cd <driver install directory>
 iii/ Run setup.bat
  iv/ Follow installation instructions.
  

b/ Method 2 -- Install from Windows Explorer

(Note: This method recommended if the <driver install directory> is stored localy on target machine. Do not use
       this method if the <driver install directory> is stored in a network location)
       
   i/ Open the <driver install directory> with Windows Explorer.
  ii/ Right click the file "setup.bat" and select "Run as administrator"
 iii/ Follow installation instructions.


2) UNINSTALLATION: The installed driver can be uninstalled from target computer by following one of the below methods:

a/ Method 1 - Uninstall from <Control Panel>\<Uninstall a program>

   i/ Start->Control Panel->Uninstall a program
  ii/ Select "Honeywell USB Serial Driver xxx". Click "Uninstall"
 iii/ Follow uninstallation instructions.

b/ Method 2 -- Uninstall from a command prompt

   i/ Open a Command Prompt with "elevated priviledge". Right click "Start->All Programs->Accessories->Command Prompt". 
      Select "Run as administrator".
  ii/ cd <driver install directory>
 iii/ Run setup.bat
  iv/ Follow uninstallation instructions.

c/ Method 3 -- Uninstall from Windows Explorer

(Note: This method recommended if the <driver install directory> is stored localy on target machine. Do not use
       this method if the <driver install directory> is stored in a network location)      
        
   i/ Open the <driver install directory> with Windows Explorer.
  ii/ Right click the file "setup.bat" and select "Run as administrator"
 iii/ Follow uninstallation instructions.

The Driver Documentations will be installed on target computer under the folder location supplied 
during the driver install wizard.

1/ "HSM USB Serial Driver Release Notes.pdf" -- List of device supported, supported OS, Known Issues.

2/ "HSM USB Serial Driver -WDReg Usage.pdf"  -- Driver installation utility. This is for customer that need to write their own
                                                installation for the driver, else normal customer will never need it.
                                               
3/ "HSM USB Serial Driver Force COM Port.pdf" -- Usage of the Driver "Force COM Port" feature.

4/ "USB SERIAL CHANGES HISTORY.pdf"         -- Enhancemences/Bugfixes.

5/ "HSM USB Serial Driver - Debug Logging.pdf" -- Instruction for enable/disable driver logging.

NOTES:
======
1/ The Microsoft Visual C++ 2010 Redistributable Package may be required to be installed on target computer.

* 32 bit OS: 
http://www.microsoft.com/en-us/download/details.aspx?id=5555

* 64 bit OS:
http://www.microsoft.com/en-us/download/details.aspx?id=14632

2/ Network location Install/Uninstall.
If the <driver install directory> is stored in a network location, it is recommended that you install/uninstall
the driver using the "command prompt" method, and do not use the "Windows Explorer" method.
You will need to map the network location to a Windows drive as shown in the below steps:

   a. Launch a command prompt with elevated priviledge
   b. Map the network location to a Windows local drive, say k:
     net use k: \\<share-network-computer>\<shared-folder> * /user:test
   c. Supply the password when prompted
   d. Check to make sure that the newly created network drive is good to go.
      net use
      The status should show "OK"
      
3/ Special character in the directory path of the <driver install directory>
If you have decided to copy the <driver install directory> to directory location on the target computer
that has special character. Install/Uninstall may not work well using the "Windows Explorer" method.
It is best to use the "command prompt" method.
For example: You have a user log account as "R&D", and copy the driver package to your desktop. So the 
<driver install directory> will be at: C:\User\R&D\Desktop\<driver install directory>

Because of the special character "&" in the directory path, running anything under there from Windows Explorer will
have an issue. In this situation, it is recommended that you use the "command prompt" method.
