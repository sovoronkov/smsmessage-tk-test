// Конфигурация для Entity Framework Core
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configuration;

public class ClientMessageHeaderConfiguration : IEntityTypeConfiguration<ClientMessageHeader>
{
    public void Configure(EntityTypeBuilder<ClientMessageHeader> builder)
    {
        builder.ToTable("ClientMessageHeader", t => t.HasComment("Сообщения клиентов главная часть"));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
               .HasColumnName("ID_MESSAGE_HEADER");

        builder.Property(x => x.DateCreation)
               .HasColumnName("DATE_CREATION")
               .IsRequired();

        builder.Property(x => x.RemoteIp)
               .HasColumnName("REMOTE_IP")
               .HasMaxLength(30);

        builder.Property(x => x.Message)
               .HasColumnName("MESSAGE")
               .IsRequired()
               .HasMaxLength(2000);

        builder.Property(x => x.Status)
               .HasColumnName("STATUS")
               .IsRequired()
               .HasConversion<int>();

        builder.Property(x => x.DateSentToProvider)
               .HasColumnName("DATE_SENT_TO_PROVIDER");

        builder.Property(x => x.ResponseResult)
               .HasColumnName("RESPONSE_RESULT");

        builder.Property(x => x.ResponseCode)
               .HasColumnName("RESPONSE_CODE");

        builder.Property(x => x.ResponseDescription)
               .HasColumnName("RESPONSE_DESCRIPTION")
               .HasMaxLength(2000);

        builder.Property(x => x.Response)
               .HasColumnName("RESPONSE")
               .HasColumnType("CLOB");

        //  // Отношение один-ко-многим
        //  builder.HasMany(x => x.ClientMessageBodies)
        //      .WithOne(x => x.ClientMessageHeader)
        //      .HasForeignKey(x => x.ClientMessageHeaderId)
        //      .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.DateCreation);
        builder.HasIndex(x => x.DateSentToProvider);
        builder.HasIndex(x => x.RemoteIp);

    }
}