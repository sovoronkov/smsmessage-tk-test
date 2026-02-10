using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configuration;

public class ClientMessageBodyConfiguration : IEntityTypeConfiguration<ClientMessageBody>
{
    public void Configure(EntityTypeBuilder<ClientMessageBody> builder)
    {
        builder.ToTable("ClientMessageBody", t => t.HasComment("Сообщения клиентов дочерняя часть"));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
               .HasColumnName("ID_MESSAGE_BODY");

        builder.Property(x => x.ClientMessageHeaderId)
               .HasColumnName("ID_MESSAGE_HEADER")
               .IsRequired();

        builder.Property(x => x.Phone)
               .HasColumnName("PHONE")
               .IsRequired()
               .HasMaxLength(30);

        builder.Property(x => x.SmsId)
               .HasColumnName("SMS_ID")
               .HasMaxLength(60);

        builder.HasOne(wp => wp.ClientMessageHeader)
              .WithMany(tp => tp.ClientMessageBodies)
              .HasForeignKey(wp => wp.ClientMessageHeaderId)
              .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.Phone);
        builder.HasIndex(x => x.SmsId);
    }
}