using System;
using System.Collections.Generic;
using DidNet.Common.Verification;
using Newtonsoft.Json;

namespace DidNet.Json.Newtonsoft.Converters.Builders
{
    public class VerificationRelationshipConverterBuilder : IJsonConverterBuilder
    {
        private readonly Dictionary<Type, JsonConverter>? typeConverters = new();

        public VerificationRelationshipConverterBuilder()
        {
            InitDefault();
        }

        private void InitDefault()
        {
            this.AddOrUpdateConverter<IAssertionMethod, AssertionMethod>();
            this.AddOrUpdateConverter<IAssertionMethod, AssertionMethod>();
            this.AddOrUpdateConverter<IAuthenticationMethod, AuthenticationMethod>();
            this.AddOrUpdateConverter<ICapabilityDelegationMethod, CapabilityDelegationMethod>();
            this.AddOrUpdateConverter<ICapabilityInvocationMethod, CapabilityInvocationMethod>();
            this.AddOrUpdateConverter<IKeyAgreementMethod, KeyAgreementMethod>();
        }

        public VerificationRelationshipConverterBuilder AddOrUpdateConverter<TInterface, TImplementation>() where TInterface : IVerificationRelationship
            where TImplementation : IVerificationRelationship
        {
            
            if (typeConverters!.ContainsKey(typeof(TInterface)))
            {
                typeConverters[typeof(TInterface)] = new VerificationRelationshipConverter<TInterface, TImplementation>();
            }
            else
            {
                typeConverters.Add(typeof(TInterface), new VerificationRelationshipConverter<TInterface, TImplementation>());
            }

            return this;
        }

        public IEnumerable<JsonConverter> GetConverters()
        {
            return typeConverters!.Values;
        }
    }
}