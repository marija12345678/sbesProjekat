﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using Contracts;
using System.IO;
using static System.Net.WebRequestMethods;
using System.IdentityModel.Policy;
using System.Security.Principal;
using System.ServiceModel.Security;
using System.Security.Cryptography.X509Certificates;
using Manager;
using System.Threading;

namespace ServiceApp
{
	public class Program
	{
		static void Main(string[] args)
		{
			string fileName = @"C:\Users\wcfservice\Downloads\sbesProjekat-main\sbesProjekat-main\baza.txt";

			string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

			NetTcpBinding binding = new NetTcpBinding();
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
			string address = "net.tcp://localhost:9999/WCFService";

			ServiceHost host = new ServiceHost(typeof(WCFService));
			host.AddServiceEndpoint(typeof(IWCFService), binding, address);

            // ovo provjeriti da li nam treba
            host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            //host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;
            //List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>();
            //policies.Add(new CustomAuthorizationPolicy());
            //host.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();

            ///Custom validation mode enables creation of a custom validator - CustomCertificateValidator
            host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
			host.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new ServiceCertValidator();
			host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
			host.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);

            // Audit
            ServiceSecurityAuditBehavior newAuditBehavior = new ServiceSecurityAuditBehavior();
			newAuditBehavior.AuditLogLocation = AuditLogLocation.Application;
			newAuditBehavior.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;
			host.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
			host.Description.Behaviors.Add(newAuditBehavior);

			Console.WriteLine("------- Servis je pokrenut od: " + WindowsIdentity.GetCurrent().Name + "\n");

			try
			{
				host.Open();
				Console.WriteLine("WCFService is opened.\n");
				//Database db = new Database();
				
				try
				{
					Audit.AuthenticationSuccess(Formatter.ParseName(WindowsIdentity.GetCurrent().Name));
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}

                FileStream file = new FileStream(fileName, FileMode.OpenOrCreate);
                Console.WriteLine("File opened");
                StreamWriter streamWriter = null;

                try
                {
                    using (streamWriter = new StreamWriter(file))
                    {
                        streamWriter.WriteLine("\nSpisak koncerata:");
                        foreach (Koncert k in Database.koncerti.Values)
                        {
                            streamWriter.WriteLine("\n\t" + k.ToString());
                        }
                        streamWriter.WriteLine("\nSpisak rezervacija:");
                        foreach (Rezervacija r in Database.rezervacije.Values)
                        {
                            streamWriter.WriteLine("\n\t" + r.ToString());

                        }
						streamWriter.WriteLine("\n\nNovi unos:");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                Console.ReadLine();
			}
			catch (Exception e)
			{
				Console.WriteLine("[ERROR] {0}", e.Message);
				Console.WriteLine("[StackTrace] {0}", e.StackTrace);
			}
			finally
			{
				Console.ReadLine();
				host.Close();
			}
		}
	}
}
